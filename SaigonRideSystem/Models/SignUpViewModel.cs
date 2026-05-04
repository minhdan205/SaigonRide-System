using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class SignUpViewModel : IValidatableObject
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Passport Number")]
        public string? Passport { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsVietnam(Country) && string.IsNullOrWhiteSpace(Passport))
            {
                yield return new ValidationResult(
                    "Passport number is required for foreign users.",
                    new[] { nameof(Passport) }
                );
            }
        }

        public static bool IsVietnam(string country)
        {
            return country.Trim().Equals("Vietnam", StringComparison.OrdinalIgnoreCase)
                || country.Trim().Equals("Viet Nam", StringComparison.OrdinalIgnoreCase)
                || country.Trim().Equals("Việt Nam", StringComparison.OrdinalIgnoreCase);
        }
    }
}