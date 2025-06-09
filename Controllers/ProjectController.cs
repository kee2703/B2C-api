using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RotaryAdminAPI.Models;
using RotaryAdminAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace RotaryAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _service.GetAllAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _service.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Project project, [FromForm] List<IFormFile>? images)
        {
            try
            {
                var created = await _service.CreateAsync(project, images);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Project project, [FromForm] List<IFormFile>? images, [FromForm] string? removedImageIds)
        {
            var removedIdsList = string.IsNullOrEmpty(removedImageIds)
                ? new List<int>()
                : removedImageIds.Split(',').Select(int.Parse).ToList();

            var updated = await _service.UpdateAsync(id, project, images, removedIdsList);
            if (updated == null) return NotFound();
            return Ok(updated);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
