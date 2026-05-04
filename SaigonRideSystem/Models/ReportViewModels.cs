namespace SaigonRideSystem.Models
{
    public class RevenueReportItem
    {
        public string VehicleCategory { get; set; } = string.Empty;
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class StationInventoryReportItem
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int CurrentInventory { get; set; }
        public decimal UtilizationRate { get; set; }
        public bool IsLowInventory { get; set; }
    }
}