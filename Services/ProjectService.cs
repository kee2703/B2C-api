using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RotaryAdminAPI.Data;
using RotaryAdminAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RotaryAdminAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectService(AppDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;

        }
        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) return string.Empty;

            return $"{request.Scheme}://{request.Host}";
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects.Include(p => p.Images).ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects.Include(p => p.Images)
                                          .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> CreateAsync(Project project, List<IFormFile>? images)
        {
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            if (images != null && images.Any())
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);

                project.Images = new List<ProjectImage>();
                string baseUrl = GetBaseUrl();
                foreach (var image in images)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    var fullImageUrl = $"{baseUrl}/uploads/{fileName}";
                    project.Images.Add(new ProjectImage { ImageUrl = fullImageUrl });
                }
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return project;
        }

        public async Task<Project> UpdateAsync(int id, Project updatedProject, List<IFormFile>? images, List<int>? removedImageIds)
        {
            var existing = await _context.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null)
                throw new Exception("Project not found");

            // Update fields
            existing.ProjectName = updatedProject.ProjectName;
            existing.Description = updatedProject.Description;
            existing.Cost = updatedProject.Cost;
            existing.Beneficiaries = updatedProject.Beneficiaries;
            existing.RotariansInvolved = updatedProject.RotariansInvolved;
            existing.ManHours = updatedProject.ManHours;
            existing.Date = updatedProject.Date;
            existing.UpdatedAt = DateTime.UtcNow;

            // Delete removed images
            // Delete removed images
            if (removedImageIds != null && removedImageIds.Any())
            {
                var toRemove = existing.Images?.Where(img => removedImageIds.Contains(img.Id)).ToList() ?? new List<ProjectImage>();
                foreach (var img in toRemove)
                {
                    if (!string.IsNullOrEmpty(img.ImageUrl))
                    {
                        var path = Path.Combine(_env.WebRootPath, img.ImageUrl);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    _context.ProjectImages.Remove(img);
                }
            }


            // Upload new images
            if (images != null && images.Any())
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);
                string baseUrl = GetBaseUrl();

                foreach (var image in images)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    existing.Images ??= new List<ProjectImage>();
                    var fullImageUrl = $"{baseUrl}/uploads/{fileName}";
                    existing.Images.Add(new ProjectImage { ImageUrl = fullImageUrl });
                }
            }

            await _context.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null) return false;

            _context.Projects.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
