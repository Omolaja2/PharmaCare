using System.ComponentModel.DataAnnotations;

namespace PharmacyApp.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string LocalGovernment { get; set; } = default!; 

        [StringLength(100)]
        public string Street { get; set; } = default!;

        [Phone]
        public string PhoneNumber { get; set; } = default!;

        [EmailAddress]
        public string Email { get; set; } = default!;

        public double Latitude { get; set; }  // optional 
        public double Longitude { get; set; } // optional 
    }
}
