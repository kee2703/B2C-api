using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RotaryAdminAPI.Models;

namespace RotaryAdminAPI.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<Project> CreateAsync(Project project, List<IFormFile>? images);
        Task<Project> UpdateAsync(int id, Project project, List<IFormFile>? images, List<int>? removedImageIds);
        Task<bool> DeleteAsync(int id);
    }
}
