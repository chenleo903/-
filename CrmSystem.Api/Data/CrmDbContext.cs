using Microsoft.EntityFrameworkCore;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.Data;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureCustomer(modelBuilder);
        ConfigureInteraction(modelBuilder);
        ConfigureUser(modelBuilder);
    }
    
    private void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            
            // Primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");
            
            // Required fields with length constraints
            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("company_name");
            
            entity.Property(e => e.ContactName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("contact_name");
            
            // Optional fields with length constraints
            entity.Property(e => e.Wechat)
                .HasMaxLength(100)
                .HasColumnName("wechat");
            
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            
            entity.Property(e => e.Industry)
                .HasMaxLength(100)
                .HasColumnName("industry");
            
            // Enum fields stored as strings
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasColumnName("source")
                .HasConversion<string?>();
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("status")
                .HasConversion<string>();
            
            // Array field for tags
            entity.Property(e => e.Tags)
                .HasColumnName("tags")
                .HasColumnType("text[]");
            
            // Score with default value
            entity.Property(e => e.Score)
                .HasColumnName("score")
                .HasDefaultValue(0);
            
            // Timestamp fields
            entity.Property(e => e.LastInteractionAt)
                .HasColumnName("last_interaction_at")
                .HasColumnType("timestamptz");
            
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()")
                .IsConcurrencyToken();
            
            // Soft delete flag
            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
            
            // Unique index on company_name and contact_name (only for non-deleted records)
            entity.HasIndex(e => new { e.CompanyName, e.ContactName })
                .IsUnique()
                .HasDatabaseName("uq_customer_company_contact")
                .HasFilter("is_deleted = false");
            
            // Performance indexes
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("idx_customers_status")
                .HasFilter("is_deleted = false");
            
            entity.HasIndex(e => e.Industry)
                .HasDatabaseName("idx_customers_industry")
                .HasFilter("is_deleted = false");
            
            entity.HasIndex(e => e.Source)
                .HasDatabaseName("idx_customers_source")
                .HasFilter("is_deleted = false");
            
            entity.HasIndex(e => e.LastInteractionAt)
                .HasDatabaseName("idx_customers_last_interaction")
                .IsDescending()
                .HasFilter("is_deleted = false");
        });
    }
    
    private void ConfigureInteraction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.ToTable("interactions");
            
            // Primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");
            
            // Foreign key to Customer
            entity.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("customer_id");
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Interactions)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Required timestamp field
            entity.Property(e => e.HappenedAt)
                .IsRequired()
                .HasColumnName("happened_at")
                .HasColumnType("timestamptz");
            
            // Enum fields stored as strings
            entity.Property(e => e.Channel)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("channel")
                .HasConversion<string>();
            
            entity.Property(e => e.Stage)
                .HasMaxLength(50)
                .HasColumnName("stage")
                .HasConversion<string?>();
            
            // Required text field
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("title");
            
            // Optional text fields with length constraints
            entity.Property(e => e.Summary)
                .HasMaxLength(2000)
                .HasColumnName("summary");
            
            entity.Property(e => e.RawContent)
                .HasMaxLength(10000)
                .HasColumnName("raw_content");
            
            entity.Property(e => e.NextAction)
                .HasMaxLength(500)
                .HasColumnName("next_action");
            
            // JSONB field for attachments
            entity.Property(e => e.Attachments)
                .HasColumnName("attachments")
                .HasColumnType("jsonb");
            
            // Timestamp fields
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()")
                .IsConcurrencyToken();
            
            // Composite index for efficient timeline queries
            entity.HasIndex(e => new { e.CustomerId, e.HappenedAt })
                .HasDatabaseName("idx_interactions_customer_happened")
                .IsDescending(false, true); // CustomerId ASC, HappenedAt DESC
        });
    }
    
    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            // Primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");
            
            // Unique username
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("user_name");
            
            entity.HasIndex(e => e.UserName)
                .IsUnique()
                .HasDatabaseName("uq_user_username");
            
            // Password hash
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            
            // Role
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("role");
            
            // Timestamp fields
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");
            
            entity.Property(e => e.LastLoginAt)
                .HasColumnName("last_login_at")
                .HasColumnType("timestamptz");
        });
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
