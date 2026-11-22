using System.Text.Json;
using LinqDeepDive.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LinqDeepDive.Api.Data;

/// <summary>
/// Mappa le cinque liste di FleetData su altrettante tabelle. I record del Domain non cambiano:
/// le liste e i record annidati (TimeSlot, DeliveryRequirement) finiscono in colonne jsonb.
/// </summary>
public sealed class CaricoDbContext(DbContextOptions<CaricoDbContext> options) : DbContext(options)
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<Restriction> Restrictions => Set<Restriction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(b =>
        {
            b.ToTable("vehicles");
            b.HasKey(v => v.Id);
            b.Property(v => v.Id).HasColumnName("id");
            b.Property(v => v.Plate).HasColumnName("plate");
            b.Property(v => v.Type).HasColumnName("type");
            b.Property(v => v.MaxPayloadKg).HasColumnName("max_payload_kg");
            b.Property(v => v.CurrentZoneId).HasColumnName("current_zone_id");
            b.Property(v => v.IsAvailable).HasColumnName("is_available");
            b.Property(v => v.AllowedZoneIds)
                .HasColumnName("allowed_zone_ids")
                .HasColumnType("jsonb")
                .HasConversion(JsonValueConverter<List<string>>())
                .Metadata.SetValueComparer(JsonValueComparer<List<string>>());
        });

        modelBuilder.Entity<Driver>(b =>
        {
            b.ToTable("drivers");
            b.HasKey(d => d.Id);
            b.Property(d => d.Id).HasColumnName("id");
            b.Property(d => d.FullName).HasColumnName("full_name");
            b.Property(d => d.LicenseType).HasColumnName("license_type");
            b.Property(d => d.AssignedVehicleId).HasColumnName("assigned_vehicle_id");
            b.Property(d => d.IsOnShift).HasColumnName("is_on_shift");
            b.Property(d => d.CertifiedZoneIds)
                .HasColumnName("certified_zone_ids")
                .HasColumnType("jsonb")
                .HasConversion(JsonValueConverter<List<string>>())
                .Metadata.SetValueComparer(JsonValueComparer<List<string>>());
        });

        modelBuilder.Entity<Delivery>(b =>
        {
            b.ToTable("deliveries");
            b.HasKey(d => d.Id);
            b.Property(d => d.Id).HasColumnName("id");
            b.Property(d => d.RecipientName).HasColumnName("recipient_name");
            b.Property(d => d.DestinationZoneId).HasColumnName("destination_zone_id");
            b.Property(d => d.WeightKg).HasColumnName("weight_kg");
            b.Property(d => d.RequestedDate)
                .HasColumnName("requested_date")
                .HasColumnType("timestamp without time zone");
            b.Property(d => d.PreferredSlot)
                .HasColumnName("preferred_slot")
                .HasColumnType("jsonb")
                .HasConversion(JsonValueConverter<TimeSlot>())
                .Metadata.SetValueComparer(JsonValueComparer<TimeSlot>());
            b.Property(d => d.Status).HasColumnName("status");
            b.Property(d => d.AssignedVehicleId).HasColumnName("assigned_vehicle_id");
            b.Property(d => d.AssignedDriverId).HasColumnName("assigned_driver_id");
            b.Property(d => d.Requirements)
                .HasColumnName("requirements")
                .HasColumnType("jsonb")
                .HasConversion(JsonValueConverter<List<DeliveryRequirement>>())
                .Metadata.SetValueComparer(JsonValueComparer<List<DeliveryRequirement>>());
        });

        modelBuilder.Entity<Zone>(b =>
        {
            b.ToTable("zones");
            b.HasKey(z => z.Id);
            b.Property(z => z.Id).HasColumnName("id");
            b.Property(z => z.Name).HasColumnName("name");
            b.Property(z => z.Region).HasColumnName("region");
            b.Property(z => z.IsRestricted).HasColumnName("is_restricted");
            b.Property(z => z.MaxConcurrentDeliveries).HasColumnName("max_concurrent_deliveries");
        });

        modelBuilder.Entity<Restriction>(b =>
        {
            b.ToTable("restrictions");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.ZoneId).HasColumnName("zone_id");
            b.Property(r => r.VehicleType).HasColumnName("vehicle_type");
            b.Property(r => r.BlockedSlot)
                .HasColumnName("blocked_slot")
                .HasColumnType("jsonb")
                .HasConversion(JsonValueConverter<TimeSlot>())
                .Metadata.SetValueComparer(JsonValueComparer<TimeSlot>());
            b.Property(r => r.Reason).HasColumnName("reason");
        });
    }

    private static ValueConverter<T, string> JsonValueConverter<T>() =>
        new(
            v => JsonSerializer.Serialize(v, JsonOptions),
            v => JsonSerializer.Deserialize<T>(v, JsonOptions)!);

    private static ValueComparer<T> JsonValueComparer<T>() =>
        new(
            (a, b) => JsonSerializer.Serialize(a, JsonOptions) == JsonSerializer.Serialize(b, JsonOptions),
            v => JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonOptions), JsonOptions)!);
}
