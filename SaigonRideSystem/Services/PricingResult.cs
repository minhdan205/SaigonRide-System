namespace SaigonRideSystem.Services
{
    public class PricingResult
    {
        public decimal OriginalFare { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalFare { get; set; }
        public bool DiscountApplied { get; set; }
        public int DurationMinutes { get; set; }
    }
}