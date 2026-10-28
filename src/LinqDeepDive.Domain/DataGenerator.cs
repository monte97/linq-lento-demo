using Bogus;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Domain;

public enum FleetSize
{
    Small,   // 50 veicoli, 200 consegne, 100 vincoli
    Medium,  // 200 veicoli, 2000 consegne, 1000 vincoli
    Large    // 500 veicoli, 5000 consegne, 2000 vincoli
}

/// <summary>
/// Parametri espliciti del generatore. Le taglie storiche della demo (Small, Medium, Large)
/// hanno sempre 20 zone; per far crescere anche le zone si usa questo record.
/// </summary>
public record FleetOptions(int Vehicles, int Deliveries, int Restrictions, int Zones = 20, int Seed = 42)
{
    public static FleetOptions From(FleetSize size, int seed = 42) => size switch
    {
        FleetSize.Small  => new FleetOptions(50, 200, 100, 20, seed),
        FleetSize.Medium => new FleetOptions(200, 2_000, 1_000, 20, seed),
        FleetSize.Large  => new FleetOptions(500, 5_000, 2_000, 20, seed),
        _ => throw new ArgumentOutOfRangeException(nameof(size))
    };

    public override string ToString() =>
        $"{Vehicles}v/{Deliveries}d/{Restrictions}r/{Zones}z";
}

public record FleetData(
    List<Vehicle> Vehicles,
    List<Driver> Drivers,
    List<Delivery> Deliveries,
    List<Zone> Zones,
    List<Restriction> Restrictions
);

public static class DataGenerator
{
    private static readonly string[] VehicleTypes = ["Van", "Truck", "Bike"];
    private static readonly string[] LicenseTypes = ["B", "C", "CE"];
    private static readonly string[] RequirementTypes = ["Refrigerated", "Fragile", "HazMat", "Oversized"];
    private static readonly string[] Regions = ["Nord", "Centro", "Sud", "Isole"];
    private static readonly string[] RestrictionReasons =
    [
        "ZTL attiva", "Limite peso", "Divieto notturno",
        "Lavori in corso", "Evento speciale", "Inquinamento"
    ];

    private static readonly TimeSlot[] StandardSlots =
    [
        new(new TimeOnly(6, 0), new TimeOnly(9, 0)),
        new(new TimeOnly(9, 0), new TimeOnly(12, 0)),
        new(new TimeOnly(12, 0), new TimeOnly(15, 0)),
        new(new TimeOnly(15, 0), new TimeOnly(18, 0)),
        new(new TimeOnly(18, 0), new TimeOnly(21, 0)),
    ];

    public static FleetData Generate(FleetSize size, int seed = 42) =>
        Generate(FleetOptions.From(size, seed));

    public static FleetData Generate(FleetOptions options)
    {
        var (vehicleCount, deliveryCount, restrictionCount) =
            (options.Vehicles, options.Deliveries, options.Restrictions);

        Randomizer.Seed = new Random(options.Seed);

        // Zone — numero esplicito (default 20)
        var zones = GenerateZones(options.Zones);
        var zoneIds = zones.Select(z => z.Id).ToList();

        // Veicoli
        var vehicleId = 0;
        var vehicleFaker = new Faker<Vehicle>()
            .CustomInstantiator(f =>
            {
                var type = f.PickRandom(VehicleTypes);
                var maxPayload = type switch
                {
                    "Bike"  => f.Random.Double(5, 30),
                    "Van"   => f.Random.Double(200, 1500),
                    "Truck" => f.Random.Double(1500, 10000),
                    _       => 500
                };
                return new Vehicle(
                    Id: ++vehicleId,
                    Plate: f.Random.Replace("??###??").ToUpper(),
                    Type: type,
                    MaxPayloadKg: Math.Round(maxPayload, 1),
                    CurrentZoneId: f.PickRandom(zoneIds),
                    IsAvailable: f.Random.Bool(0.7f),
                    AllowedZoneIds: f.PickRandom(zoneIds, Math.Min(f.Random.Int(3, 10), zoneIds.Count)).ToList()
                );
            });

        var vehicles = vehicleFaker.Generate(vehicleCount);

        // Driver — circa 1.2x veicoli
        var driverCount = (int)(vehicleCount * 1.2);
        var driverId = 0;
        var assignableVehicleIds = vehicles.Select(v => v.Id).ToList();
        var driverFaker = new Faker<Driver>()
            .CustomInstantiator(f =>
            {
                int? assignedVehicle = f.Random.Bool(0.6f)
                    ? f.PickRandom(assignableVehicleIds)
                    : null;
                return new Driver(
                    Id: ++driverId,
                    FullName: f.Name.FullName(),
                    LicenseType: f.PickRandom(LicenseTypes),
                    AssignedVehicleId: assignedVehicle,
                    IsOnShift: f.Random.Bool(0.65f),
                    CertifiedZoneIds: f.PickRandom(zoneIds, Math.Min(f.Random.Int(2, 8), zoneIds.Count)).ToList()
                );
            });

        var drivers = driverFaker.Generate(driverCount);

        // Deliveries
        var delivId = 0;
        var deliveryFaker = new Faker<Delivery>()
            .CustomInstantiator(f =>
            {
                var status = f.PickRandom<DeliveryStatus>();
                var isAssigned = status != DeliveryStatus.Pending;
                var reqCount = f.Random.Int(0, 3);
                var reqs = Enumerable.Range(0, reqCount)
                    .Select(_ => new DeliveryRequirement(
                        f.PickRandom(RequirementTypes),
                        f.Random.Bool() ? "true" : f.Random.Replace("##.#")))
                    .ToList();

                return new Delivery(
                    Id: ++delivId,
                    RecipientName: f.Name.FullName(),
                    DestinationZoneId: f.PickRandom(zoneIds),
                    WeightKg: Math.Round(f.Random.Double(0.5, 500), 1),
                    RequestedDate: f.Date.Between(
                        DateTime.Today.AddDays(-30),
                        DateTime.Today.AddDays(7)),
                    PreferredSlot: f.PickRandom(StandardSlots),
                    Status: status,
                    AssignedVehicleId: isAssigned ? f.PickRandom(vehicles).Id : null,
                    AssignedDriverId: isAssigned ? f.PickRandom(drivers).Id : null,
                    Requirements: reqs
                );
            });

        var deliveries = deliveryFaker.Generate(deliveryCount);

        // Restrictions
        var restrictId = 0;
        var restrictionFaker = new Faker<Restriction>()
            .CustomInstantiator(f => new Restriction(
                Id: ++restrictId,
                ZoneId: f.PickRandom(zoneIds),
                VehicleType: f.PickRandom(VehicleTypes),
                BlockedSlot: f.PickRandom(StandardSlots),
                Reason: f.PickRandom(RestrictionReasons)
            ));

        var restrictions = restrictionFaker.Generate(restrictionCount);

        return new FleetData(vehicles, drivers, deliveries, zones, restrictions);
    }

    private static List<Zone> GenerateZones(int count)
    {
        var faker = new Faker("it");
        return Enumerable.Range(1, count).Select(i =>
        {
            var region = Regions[(i - 1) * Regions.Length / count];
            return new Zone(
                Id: $"ZONE-{i:D3}",
                Name: $"Zona {faker.Address.City()}",
                Region: region,
                IsRestricted: faker.Random.Bool(0.3f),
                MaxConcurrentDeliveries: faker.Random.Int(5, 50)
            );
        }).ToList();
    }
}
