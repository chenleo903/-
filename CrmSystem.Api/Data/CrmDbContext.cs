using Microsoft.EntityFrameworkCore;

namespace CrmSystem.Api.Data;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
    {
    }

    // DbSets will be added when entities are created
    // public DbSet<Customer> Customers { get; set; }
    // public DbSet<Interaction> Interactions { get; set; }
    // public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Entity configurations will be added when entities are created
    }

    public override int SaveChanges()
    {
        ConvertDateTimeOffsetsToUtc();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDateTimeOffsetsToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically converts all DateTimeOffset values to UTC before saving
    /// This ensures all timestamps are stored in UTC timezone
    /// </summary>
    private void ConvertDateTimeOffsetsToUtc()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.ClrType == typeof(DateTimeOffset) ||
                    property.Metadata.ClrType == typeof(DateTimeOffset?))
                {
                    if (property.CurrentValue is DateTimeOffset dto)
                    {
                        property.CurrentValue = dto.ToUniversalTime();
                    }
                }
            }
        }
    }
}
