using SaigonRideSystem.Models;
using SaigonRideSystem.Services;
using Xunit;

namespace SaigonRideSystem.Tests.Services
{
    public class PricingServiceTests
    {
        private readonly PricingService _pricingService;

        public PricingServiceTests()
        {
            _pricingService = new PricingService();
        }

        [Fact]
        public void CalculateFare_StandardBike_Charges500VndPerMinute()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Act
            var result = _pricingService.CalculateFare(
                VehicleCategory.StandardBike,
                startTime,
                endTime,
                returnStationCapacity: 10,
                returnStationCurrentInventory: 2
            );

            // Assert
            Assert.Equal(10, result.DurationMinutes);
            Assert.Equal(5000m, result.OriginalFare);
            Assert.False(result.DiscountApplied);
            Assert.Equal(0m, result.DiscountAmount);
            Assert.Equal(5000m, result.FinalFare);
        }

        [Fact]
        public void CalculateFare_EScooter_Charges1500VndPerMinute()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Act
            var result = _pricingService.CalculateFare(
                VehicleCategory.EScooter,
                startTime,
                endTime,
                returnStationCapacity: 10,
                returnStationCurrentInventory: 2
            );

            // Assert
            Assert.Equal(10, result.DurationMinutes);
            Assert.Equal(15000m, result.OriginalFare);
            Assert.False(result.DiscountApplied);
            Assert.Equal(0m, result.DiscountAmount);
            Assert.Equal(15000m, result.FinalFare);
        }

        [Fact]
        public void CalculateFare_WhenInventoryBelow20Percent_Applies15PercentDiscount()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Capacity = 10, Inventory = 1 => 10% < 20%
            // Standard Bike fare = 10 * 500 = 5000
            // Discount = 5000 * 15% = 750
            // Final = 4250

            // Act
            var result = _pricingService.CalculateFare(
                VehicleCategory.StandardBike,
                startTime,
                endTime,
                returnStationCapacity: 10,
                returnStationCurrentInventory: 1
            );

            // Assert
            Assert.True(result.DiscountApplied);
            Assert.Equal(5000m, result.OriginalFare);
            Assert.Equal(750m, result.DiscountAmount);
            Assert.Equal(4250m, result.FinalFare);
        }

        [Fact]
        public void CalculateFare_WhenInventoryEquals20Percent_DoesNotApplyDiscount()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Capacity = 10, Inventory = 2 => exactly 20%
            // Rule is inventory < 20%, so no discount.

            // Act
            var result = _pricingService.CalculateFare(
                VehicleCategory.StandardBike,
                startTime,
                endTime,
                returnStationCapacity: 10,
                returnStationCurrentInventory: 2
            );

            // Assert
            Assert.False(result.DiscountApplied);
            Assert.Equal(5000m, result.OriginalFare);
            Assert.Equal(0m, result.DiscountAmount);
            Assert.Equal(5000m, result.FinalFare);
        }

        [Fact]
        public void CalculateFare_WhenInventoryAbove20Percent_DoesNotApplyDiscount()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Capacity = 10, Inventory = 3 => 30% > 20%

            // Act
            var result = _pricingService.CalculateFare(
                VehicleCategory.StandardBike,
                startTime,
                endTime,
                returnStationCapacity: 10,
                returnStationCurrentInventory: 3
            );

            // Assert
            Assert.False(result.DiscountApplied);
            Assert.Equal(5000m, result.FinalFare);
        }

        [Fact]
        public void CalculateFare_RoundsDurationUpToNextMinute()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 30);

            // 10 minutes 30 seconds should be rounded up to 11 minutes.

            // Act
            var result = _pricingService.CalculateFare(
                VehicleCategory.StandardBike,
                startTime,
                endTime,
                returnStationCapacity: 10,
                returnStationCurrentInventory: 2
            );

            // Assert
            Assert.Equal(11, result.DurationMinutes);
            Assert.Equal(5500m, result.FinalFare);
        }

        [Fact]
        public void CalculateFare_WhenEndTimeBeforeStartTime_ThrowsArgumentException()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 10, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 0, 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _pricingService.CalculateFare(
                    VehicleCategory.StandardBike,
                    startTime,
                    endTime,
                    returnStationCapacity: 10,
                    returnStationCurrentInventory: 2
                )
            );
        }

        [Fact]
        public void CalculateFare_WhenCapacityIsZero_ThrowsArgumentException()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _pricingService.CalculateFare(
                    VehicleCategory.StandardBike,
                    startTime,
                    endTime,
                    returnStationCapacity: 0,
                    returnStationCurrentInventory: 0
                )
            );
        }

        [Fact]
        public void CalculateFare_WhenInventoryGreaterThanCapacity_ThrowsArgumentException()
        {
            // Arrange
            var startTime = new DateTime(2026, 5, 10, 10, 0, 0);
            var endTime = new DateTime(2026, 5, 10, 10, 10, 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _pricingService.CalculateFare(
                    VehicleCategory.StandardBike,
                    startTime,
                    endTime,
                    returnStationCapacity: 10,
                    returnStationCurrentInventory: 11
                )
            );
        }
    }
}