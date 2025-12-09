# CRM System - Setup Guide

## Project Initialization Complete ✅

The ASP.NET Core 8 Web API project structure has been successfully initialized with all required dependencies and folder structure.

## What Has Been Created

### 1. Solution Structure
- `CrmSystem.sln` - Solution file containing both API and Tests projects
- `.gitignore` - Comprehensive .NET gitignore configuration

### 2. Backend API Project (`CrmSystem.Api/`)

#### Project File
- `CrmSystem.Api.csproj` with all required NuGet packages:
  - Entity Framework Core 8.0.0
  - Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0
  - FluentValidation.AspNetCore 11.3.0
  - Serilog.AspNetCore 8.0.0
  - BCrypt.Net-Next 4.0.3
  - JWT Authentication packages

#### Configuration Files
- `appsettings.json` - Main configuration with database connection, auth settings
- `appsettings.Development.json` - Development-specific settings
- `Program.cs` - Application entry point (basic setup)

#### Folder Structure
- `Controllers/` - API controllers (empty, ready for implementation)
- `Services/` - Business logic layer (empty)
- `Repositories/` - Data access layer (empty)
- `Models/` - Entity models (empty)
- `DTOs/` - Data Transfer Objects (empty)
- `Middleware/` - Custom middleware (empty)
- `Data/` - DbContext and database configuration (empty)
- `Exceptions/` - Custom exception classes (empty)
- `Helpers/` - Utility classes (empty)

### 3. Test Project (`CrmSystem.Tests/`)

#### Project File
- `CrmSystem.Tests.csproj` with testing packages:
  - xUnit 2.6.2
  - FsCheck 2.16.6 (for property-based testing)
  - FsCheck.Xunit 2.16.6
  - Moq 4.20.70
  - Microsoft.AspNetCore.Mvc.Testing 8.0.0
  - Testcontainers.PostgreSql 3.6.0

#### Folder Structure
- `UnitTests/` - Unit tests (empty)
- `PropertyTests/` - Property-based tests using FsCheck (empty)
- `IntegrationTests/` - Integration tests (empty)
- `Generators/` - FsCheck generators for test data (empty)

### 4. Documentation
- `README.md` - Comprehensive project documentation
- `SETUP.md` - This file

## Next Steps

### Prerequisites to Install

Since .NET SDK is not currently installed on this system, you'll need to:

1. **Install .NET 8 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version`

2. **Install PostgreSQL 16**
   - Download from: https://www.postgresql.org/download/
   - Or use Docker: `docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:16`

3. **Install Docker & Docker Compose** (optional, for containerized deployment)
   - Download from: https://www.docker.com/products/docker-desktop

### After Installing .NET SDK

1. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Verify the test project**
   ```bash
   dotnet test
   ```

### Continue with Task 2

Once .NET SDK is installed and packages are restored, you can proceed to:
- **Task 2**: Configure database and EF Core
  - Set up DbContext
  - Configure PostgreSQL connection
  - Set up time zone handling

## Configuration Notes

### Database Connection
The default connection string in `appsettings.json` is:
```
Host=localhost;Port=5432;Database=crm;Username=postgres;Password=postgres
```

Update this based on your PostgreSQL setup.

### Authentication (Optional)
Authentication is disabled by default (`EnableAuth: false`). To enable:
1. Set `ENABLE_AUTH=true`
2. Provide `ADMIN_USERNAME` and `ADMIN_PASSWORD`
3. Provide `JWT_SECRET` (at least 32 characters)

### Auto Migration
Database auto-migration is disabled by default (`AutoMigrate: false`). Enable it in production environments with caution.

## Project Status

✅ Task 1: Initialize backend project structure - **COMPLETE**
⏳ Task 2: Configure database and EF Core - **NEXT**

All folder structures are in place and ready for implementation according to the design document.
