namespace LinqDeepDive.Domain.Models;

public record Zone(
    string Id,
    string Name,
    string Region,
    bool IsRestricted,
    int MaxConcurrentDeliveries
);
