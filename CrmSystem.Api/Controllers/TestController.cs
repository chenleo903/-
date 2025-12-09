using Microsoft.AspNetCore.Mvc;
using CrmSystem.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CrmSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly CrmDbContext _context;
    private readonly ILogger<TestController> _logger;

    public TestController(CrmDbContext context, ILogger<TestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 测试数据库连接
    /// </summary>
    [HttpGet("db-connection")]
    public async Task<IActionResult> TestDatabaseConnection()
    {
        try
        {
            _logger.LogInformation("Testing database connection...");
            
            // 尝试连接数据库
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                _logger.LogInformation("Database connection successful");
                return Ok(new
                {
                    success = true,
                    message = "Database connection successful",
                    databaseProvider = _context.Database.ProviderName
                });
            }
            else
            {
                _logger.LogWarning("Database connection failed");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Cannot connect to database"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing database connection");
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// 测试 JSON 序列化配置
    /// </summary>
    [HttpGet("json-serialization")]
    public IActionResult TestJsonSerialization()
    {
        _logger.LogInformation("Testing JSON serialization configuration");
        
        var testData = new TestResponse
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            Status = TestStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.Now, // 非 UTC，应该被转换
            NullableDate = null,
            Score = 85
        };

        return Ok(testData);
    }

    /// <summary>
    /// 测试日志记录
    /// </summary>
    [HttpGet("logging")]
    public IActionResult TestLogging()
    {
        _logger.LogDebug("This is a DEBUG log");
        _logger.LogInformation("This is an INFO log");
        _logger.LogWarning("This is a WARNING log");
        _logger.LogError("This is an ERROR log");

        return Ok(new
        {
            success = true,
            message = "Check console and logs/crm-*.log file for log entries"
        });
    }
}

public class TestResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TestStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? NullableDate { get; set; }
    public int Score { get; set; }
}

public enum TestStatus
{
    Active,
    Inactive,
    Pending
}
