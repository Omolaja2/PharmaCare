using System.ComponentModel.DataAnnotations;

namespace PharmacyApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "Patient";

        public ICollection<Order>? Orders { get; set; }
    }
}