using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class Rental
    {
        public int RentalId { get; set; }

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        [Required]
        public string VehicleId { get; set; } = string.Empty;

        public Vehicle? Vehicle { get; set; }

        [Required]
        public int StartStationId { get; set; }

        public Station? StartStation { get; set; }

        public int? ReturnStationId { get; set; }

        public Station? ReturnStation { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public decimal TotalFare { get; set; }

        public bool DiscountApplied { get; set; }

        public decimal DiscountAmount { get; set; }

        public int? DiscountCodeId { get; set; }

        public DiscountCode? DiscountCode { get; set; }

        [StringLength(30)]
        public string? AppliedDiscountCode { get; set; }

        public int? CodeDiscountPercent { get; set; }

        public decimal CodeDiscountAmount { get; set; }

        [Required]
        public RentalStatus Status { get; set; }

        public Payment? Payment { get; set; }
    }
}