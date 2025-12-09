using System.Text.Json.Serialization;
using CrmSystem.Api.Data;
using CrmSystem.Api.Helpers;
using CrmSystem.Api.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/crm-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<CrmDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configure JSON serialization options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Add custom DateTimeOffset converters for ISO 8601 format
        options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableDateTimeOffsetConverter());
        
        // Configure enum serialization as strings
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        
        // Ignore null values in responses
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        
        // Use camelCase for property names
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Register MigrationService
builder.Services.AddScoped<MigrationService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Handle database migrations
var autoMigrate = builder.Configuration.GetValue<bool>("Migration:AutoMigrate", false);

if (autoMigrate)
{
    Log.Information("AUTO_MIGRATE is enabled. Attempting to apply database migrations...");
    
    try
    {
        using var scope = app.Services.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
        await migrationService.MigrateAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to apply database migrations. Application will not start.");
        return; // Prevent application startup
    }
}
else
{
    Log.Information("AUTO_MIGRATE is disabled. Skipping automatic database migrations.");
    Log.Information("To apply migrations manually, run: dotnet ef database update");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting CRM System API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
