using CrmSystem.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CrmSystem.Api.Services;

/// <summary>
/// Service for managing database migrations with PostgreSQL advisory locks
/// to ensure only one instance performs migrations in multi-instance deployments.
/// </summary>
public class MigrationService
{
    private const long MigrationLockId = 123456789; // Unique ID for advisory lock
    private readonly CrmDbContext _context;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(CrmDbContext context, ILogger<MigrationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Applies pending database migrations with advisory lock protection.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="InvalidOperationException">Thrown when migration fails</exception>
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            // Try to acquire advisory lock (non-blocking)
            using var tryLockCommand = connection.CreateCommand();
            tryLockCommand.CommandText = $"SELECT pg_try_advisory_lock({MigrationLockId})";
            var lockAcquired = (bool)(await tryLockCommand.ExecuteScalarAsync(cancellationToken))!;

            if (!lockAcquired)
            {
                _logger.LogInformation("Another instance is running migrations. Waiting for lock...");

                // Block and wait for lock (with timeout)
                using var lockCommand = connection.CreateCommand();
                lockCommand.CommandText = $"SELECT pg_advisory_lock({MigrationLockId})";
                lockCommand.CommandTimeout = 30; // 30 seconds timeout

                try
                {
                    await lockCommand.ExecuteNonQueryAsync(cancellationToken);
                    _logger.LogInformation("Migration lock acquired after waiting");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to acquire migration lock within timeout period");
                    throw new InvalidOperationException("Failed to acquire migration lock. Another instance may be stuck.", ex);
                }
            }
            else
            {
                _logger.LogInformation("Migration lock acquired immediately");
            }

            try
            {
                // Check if there are pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
                var pendingCount = pendingMigrations.Count();

                if (pendingCount > 0)
                {
                    _logger.LogInformation("Applying {Count} pending database migration(s)...", pendingCount);
                    
                    // Execute migrations
                    await _context.Database.MigrateAsync(cancellationToken);
                    
                    _logger.LogInformation("Database migrations completed successfully");
                }
                else
                {
                    _logger.LogInformation("Database is up to date. No migrations to apply.");
                }
            }
            finally
            {
                // Release the advisory lock
                using var unlockCommand = connection.CreateCommand();
                unlockCommand.CommandText = $"SELECT pg_advisory_unlock({MigrationLockId})";
                await unlockCommand.ExecuteNonQueryAsync(cancellationToken);
                _logger.LogInformation("Migration lock released");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Database migration failed");
            throw new InvalidOperationException("Database migration failed. Please check the logs and database state.", ex);
        }
    }
}
