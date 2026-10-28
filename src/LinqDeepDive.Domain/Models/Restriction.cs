namespace LinqDeepDive.Domain.Models;

public record Restriction(
    int Id,
    string ZoneId,
    string VehicleType,      // which vehicle type is restricted
    TimeSlot BlockedSlot,
    string Reason
);
