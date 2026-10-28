namespace LinqDeepDive.Domain.Models;

public record DeliveryRequirement(
    string Type,     // "Refrigerated", "Fragile", "HazMat", "Oversized"
    string Value
);
