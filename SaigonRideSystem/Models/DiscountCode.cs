using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class DiscountCode : IValidatableObject
    {
        public int DiscountCodeId { get; set; }

        [Required]
        [StringLength(100)]
        public string CodeName { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Discount Percent")]
        public int DiscountPercent { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int[] allowedValues = { 30, 50, 70, 100 };

            if (!allowedValues.Contains(DiscountPercent))
            {
                yield return new ValidationResult(
                    "Discount percent must be 30%, 50%, 70%, or 100%.",
                    new[] { nameof(DiscountPercent) }
                );
            }
        }
    }
}