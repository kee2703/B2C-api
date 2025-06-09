
namespace RotaryAdminAPI.Models
{

    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Additionalph { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string AdminUsername { get; set; } = string.Empty;

    }
}