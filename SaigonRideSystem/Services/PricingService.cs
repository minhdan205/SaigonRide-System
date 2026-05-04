using SaigonRideSystem.Models;

namespace SaigonRideSystem.Services
{
    public class PricingService
    {
        private const decimal StandardBikeRate = 500m;
        private const decimal EScooterRate = 1500m;
        private const decimal DiscountRate = 0.15m;

        public PricingResult CalculateFare(
            VehicleCategory category,
            DateTime startTime,
            DateTime endTime,
            int returnStationCapacity,
            int returnStationCurrentInventory)
        {
            if (endTime <= startTime)
            {
                throw new ArgumentException("End time must be later than start time.");
            }

            if (returnStationCapacity <= 0)
            {
                throw new ArgumentException("Station capacity must be greater than 0.");
            }

            if (returnStationCurrentInventory < 0 || returnStationCurrentInventory > returnStationCapacity)
            {
                throw new ArgumentException("Station inventory is invalid.");
            }

            int durationMinutes = (int)Math.Ceiling((endTime - startTime).TotalMinutes);

            decimal ratePerMinute = category switch
            {
                VehicleCategory.StandardBike => StandardBikeRate,
                VehicleCategory.EScooter => EScooterRate,
                _ => throw new ArgumentException("Invalid vehicle category.")
            };

            decimal originalFare = durationMinutes * ratePerMinute;

            bool isLowInventoryStation =
                returnStationCurrentInventory < returnStationCapacity * 0.20m;

            decimal discountAmount = isLowInventoryStation
                ? originalFare * DiscountRate
                : 0m;

            decimal finalFare = originalFare - discountAmount;

            return new PricingResult
            {
                DurationMinutes = durationMinutes,
                OriginalFare = originalFare,
                DiscountApplied = isLowInventoryStation,
                DiscountAmount = discountAmount,
                FinalFare = finalFare
            };
        }
    }
}