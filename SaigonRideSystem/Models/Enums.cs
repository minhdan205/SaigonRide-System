namespace SaigonRideSystem.Models
{
    public enum VehicleCategory
    {
        StandardBike,
        EScooter
    }

    public enum VehicleStatus
    {
        Available,
        InTransit,
        Maintenance
    }

    public enum UserType
    {
        Admin,
        Local,
        Tourist
    }

    public enum RentalStatus
    {
        Active,
        Completed,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        MoMo,
        VNPay,
        ApplePay,
        PayPal
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }
}