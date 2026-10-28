namespace LinqDeepDive.Domain.Models;

public record Delivery(
    int Id,
    string RecipientName,
    string DestinationZoneId,
    double WeightKg,
    DateTime RequestedDate,
    TimeSlot PreferredSlot,
    DeliveryStatus Status,
    int? AssignedVehicleId,
    int? AssignedDriverId,
    List<DeliveryRequirement> Requirements
);

public enum DeliveryStatus
{
    Pending,
    Assigned,
    InTransit,
    Delivered,
    Failed
}
