using Microsoft.AspNetCore.Http;

namespace RotaryAdminAPI.Models
{
	public class ProjectUploadModel
	{
		public string ProjectName { get; set; } = "";
		public string Description { get; set; } = "";
		public IFormFile File { get; set; } = default!;
	}
}
