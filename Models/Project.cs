using System;
using System.Collections.Generic;

namespace RotaryAdminAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public int Beneficiaries { get; set; }
        public int RotariansInvolved { get; set; }
        public int ManHours { get; set; }
        public DateTime Date { get; set; }

        public ICollection<ProjectImage>? Images { get; set; } =  new List<ProjectImage>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
