using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class Vehicle
    {
        [Key]
        [Required]
        [StringLength(30)]
        public string VehicleId { get; set; } = string.Empty;

        [Required]
        public VehicleCategory Category { get; set; }

        [Required]
        public VehicleStatus Status { get; set; }

        [Required]
        public int StationId { get; set; }

        public Station? Station { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}