namespace LinqDeepDive.Domain.Models;

public record Vehicle(
    int Id,
    string Plate,
    string Type,         // "Van", "Truck", "Bike"
    double MaxPayloadKg,
    string CurrentZoneId,
    bool IsAvailable,
    List<string> AllowedZoneIds
);
