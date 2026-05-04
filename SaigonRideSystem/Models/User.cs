using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class User : IValidatableObject
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        [StringLength(100)]
        public string? Passport { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserType == UserType.Tourist && string.IsNullOrWhiteSpace(Passport))
            {
                yield return new ValidationResult(
                    "Passport is required for tourist users.",
                    new[] { nameof(Passport) }
                );
            }
        }
    }
}