using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class Station : IValidatableObject
    {
        public int StationId { get; set; }

        [Required]
        [StringLength(100)]
        public string StationName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Current inventory cannot be negative.")]
        public int CurrentInventory { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public ICollection<Rental> StartRentals { get; set; } = new List<Rental>();

        public ICollection<Rental> ReturnRentals { get; set; } = new List<Rental>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentInventory > Capacity)
            {
                yield return new ValidationResult(
                    "Current inventory must not exceed capacity.",
                    new[] { nameof(CurrentInventory) }
                );
            }
        }
    }
}