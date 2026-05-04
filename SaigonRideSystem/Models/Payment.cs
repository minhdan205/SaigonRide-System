using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        [Required]
        public int RentalId { get; set; }

        public Rental? Rental { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }
    }
}