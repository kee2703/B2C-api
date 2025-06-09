using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotaryAdminAPI.Data;
using RotaryAdminAPI.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RotaryAdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BannerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Banner
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllBanners()
        {
            var banners = await _context.Banner.ToListAsync();
            return Ok(banners);
        }

        // GET: api/Banner/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannerById(int id)
        {
            var banner = await _context.Banner.FindAsync(id);
            if (banner == null)
                return NotFound(new { message = "Banner not found" });

            return Ok(banner);
        }

        // POST: api/Banner/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadBanner([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Only image files are allowed." });

            try
            {
                var folderName = "banners";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}"; 
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var publicPath = $"/{folderName}/{uniqueFileName}";
                var banner = new Banner
                {
                    FileName = uniqueFileName,
                    FilePath = publicPath,
                    CreatedAt = DateTime.Now
                };

                _context.Banner.Add(banner);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Banner uploaded successfully.",
                    banner = new
                    {
                        banner.Id,
                        banner.FileName,
                        banner.FilePath, 
                        ImageUrl = $"{Request.Scheme}://{Request.Host}{publicPath}"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading banner.", error = ex.Message });
            }
        }
        // PUT: api/Banner/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBanner(int id, [FromBody] Banner updatedBanner)
        {
            if (id != updatedBanner.Id)
                return BadRequest(new { message = "ID mismatch" });

            var banner = await _context.Banner.FindAsync(id);
            if (banner == null)
                return NotFound(new { message = "Banner not found" });

            banner.FileName = updatedBanner.FileName;
            // Optional: Update CreatedAt or other properties

            await _context.SaveChangesAsync();
            return Ok(new { message = "Banner updated successfully." });
        }

        // DELETE: api/Banner/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.Banner.FindAsync(id);
            if (banner == null)
                return NotFound(new { message = "Banner not found" });

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "banners", banner.FileName ?? "");
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Banner.Remove(banner);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Banner deleted successfully." });
        }
    }
}
