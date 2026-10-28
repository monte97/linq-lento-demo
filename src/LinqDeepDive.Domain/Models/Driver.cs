namespace LinqDeepDive.Domain.Models;

public record Driver(
    int Id,
    string FullName,
    string LicenseType,   // "B", "C", "CE"
    int? AssignedVehicleId,
    bool IsOnShift,
    List<string> CertifiedZoneIds
);
