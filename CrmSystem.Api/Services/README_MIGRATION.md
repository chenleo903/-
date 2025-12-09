# Database Migration Service

## Overview

The `MigrationService` provides safe database migration execution with PostgreSQL advisory locks to prevent concurrent migration attempts in multi-instance deployments.

## Features

- **Advisory Lock Protection**: Uses PostgreSQL's `pg_advisory_lock` to ensure only one instance performs migrations
- **Timeout Handling**: 30-second timeout when waiting for lock acquisition
- **Comprehensive Logging**: Detailed logging of migration process
- **Error Handling**: Proper error handling with informative exception messages

## Configuration

### Enable Auto-Migration

In `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Migration": {
    "AutoMigrate": true
  }
}
```

Or via environment variable:
```bash
Migration__AutoMigrate=true
```

### Disable Auto-Migration (Production Recommended)

```json
{
  "Migration": {
    "AutoMigrate": false
  }
}
```

## Usage

### Automatic Migration (Development)

When `AutoMigrate` is set to `true`, migrations run automatically on application startup:

```bash
dotnet run
```

The service will:
1. Attempt to acquire an advisory lock
2. Check for pending migrations
3. Apply migrations if any exist
4. Release the lock
5. Continue with application startup

### Manual Migration (Production)

When `AutoMigrate` is set to `false`, run migrations manually:

```bash
dotnet ef database update
```

## Multi-Instance Deployment

In scenarios with multiple API instances (e.g., Kubernetes, Docker Swarm):

1. **First Instance**: Acquires lock immediately and runs migrations
2. **Other Instances**: Wait for lock (up to 30 seconds), then proceed once migrations complete
3. **All Instances**: Continue startup after migrations are done

## Advisory Lock Details

- **Lock ID**: `123456789` (constant defined in `MigrationService`)
- **Lock Type**: PostgreSQL advisory lock (session-level)
- **Timeout**: 30 seconds for blocking wait
- **Scope**: Database-wide (all instances connecting to the same database)

## Error Scenarios

### Lock Timeout

If another instance holds the lock for more than 30 seconds:

```
Failed to acquire migration lock. Another instance may be stuck.
```

**Resolution**: Check if another instance is stuck, restart if necessary.

### Migration Failure

If migrations fail to apply:

```
Database migration failed. Please check the logs and database state.
```

**Resolution**: 
1. Check application logs for detailed error
2. Verify database connectivity
3. Check migration scripts for errors
4. Manually fix database state if needed

### Startup Prevention

If migrations fail and `AutoMigrate` is enabled, the application will not start to prevent running with an incorrect database schema.

## Logging

The service logs the following events:

- Lock acquisition (immediate or after waiting)
- Number of pending migrations
- Migration completion
- Lock release
- Errors and warnings

Example log output:

```
[Information] AUTO_MIGRATE is enabled. Attempting to apply database migrations...
[Information] Migration lock acquired immediately
[Information] Applying 2 pending database migration(s)...
[Information] Database migrations completed successfully
[Information] Migration lock released
```

## Best Practices

### Development
- Enable `AutoMigrate` for convenience
- Migrations run automatically on each startup

### Production
- Disable `AutoMigrate` for control
- Run migrations manually during deployment
- Use CI/CD pipeline to apply migrations
- Test migrations in staging environment first

### Multi-Instance Production
- If using `AutoMigrate`, ensure all instances connect to the same database
- Monitor logs to verify only one instance runs migrations
- Consider using a dedicated migration job instead of auto-migration

## Implementation Details

The service uses:
- `pg_try_advisory_lock()`: Non-blocking lock attempt
- `pg_advisory_lock()`: Blocking lock with timeout
- `pg_advisory_unlock()`: Lock release
- `EF Core Migrations`: Actual migration execution

## Requirements

- PostgreSQL 9.1 or higher (for advisory locks)
- Entity Framework Core 8.0 or higher
- Npgsql provider

## Related Files

- `CrmSystem.Api/Services/MigrationService.cs`: Service implementation
- `CrmSystem.Api/Program.cs`: Service integration
- `appsettings.json`: Configuration
- `CrmSystem.Api/Migrations/`: Migration files
