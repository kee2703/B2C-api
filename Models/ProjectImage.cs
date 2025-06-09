using System;
using System.Text.Json.Serialization;

namespace RotaryAdminAPI.Models
{
    public class ProjectImage
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project? Project { get; set; }
    }
}
