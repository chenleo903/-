using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CrmSystem.Api.Middleware;

/// <summary>
/// 请求日志中间件
/// 记录请求路径、方法、状态码、处理时间，并对敏感字段进行脱敏
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    // Sensitive field names to mask in request body
    private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "phone",
        "email",
        "wechat"
    };

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        // Read and log request body for POST/PUT/PATCH requests
        string? requestBody = null;
        if (HttpMethods.IsPost(requestMethod) || 
            HttpMethods.IsPut(requestMethod) || 
            HttpMethods.IsPatch(requestMethod))
        {
            context.Request.EnableBuffering();
            requestBody = await ReadAndMaskRequestBodyAsync(context.Request);
            context.Request.Body.Position = 0;
        }

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (requestBody != null)
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms - Body: {RequestBody}",
                    requestMethod,
                    requestPath,
                    statusCode,
                    elapsedMs,
                    requestBody);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    requestMethod,
                    requestPath,
                    statusCode,
                    elapsedMs);
            }
        }
    }

    private async Task<string?> ReadAndMaskRequestBodyAsync(HttpRequest request)
    {
        try
        {
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(body))
                return null;

            return MaskSensitiveData(body);
        }
        catch
        {
            return "[Unable to read request body]";
        }
    }

    /// <summary>
    /// Masks sensitive data in JSON body
    /// </summary>
    private string MaskSensitiveData(string jsonBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonBody);
            var maskedObject = MaskJsonElement(doc.RootElement);
            return JsonSerializer.Serialize(maskedObject, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch
        {
            // If not valid JSON, return as-is
            return jsonBody;
        }
    }

    private object? MaskJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var property in element.EnumerateObject())
                {
                    if (SensitiveFields.Contains(property.Name) && 
                        property.Value.ValueKind == JsonValueKind.String)
                    {
                        var value = property.Value.GetString();
                        dict[property.Name] = MaskValue(property.Name, value);
                    }
                    else
                    {
                        dict[property.Name] = MaskJsonElement(property.Value);
                    }
                }
                return dict;

            case JsonValueKind.Array:
                var list = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(MaskJsonElement(item));
                }
                return list;

            case JsonValueKind.String:
                return element.GetString();

            case JsonValueKind.Number:
                if (element.TryGetInt64(out var longValue))
                    return longValue;
                return element.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
            default:
                return null;
        }
    }

    /// <summary>
    /// Masks a sensitive value based on field type
    /// </summary>
    private static string? MaskValue(string fieldName, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return fieldName.ToLowerInvariant() switch
        {
            "phone" => MaskPhone(value),
            "email" => MaskEmail(value),
            "wechat" => MaskWechat(value),
            _ => "****"
        };
    }

    /// <summary>
    /// Masks phone number - keeps only last 4 digits
    /// Example: 13812345678 -> *******5678
    /// </summary>
    public static string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 4)
            return "****";

        var lastFour = phone[^4..];
        var maskedPart = new string('*', phone.Length - 4);
        return maskedPart + lastFour;
    }

    /// <summary>
    /// Masks email - keeps first 2 characters before @ and domain
    /// Example: john.doe@example.com -> jo****@example.com
    /// </summary>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return "****";

        var atIndex = email.IndexOf('@');
        if (atIndex < 0)
            return "****";

        var localPart = email[..atIndex];
        var domain = email[atIndex..];

        if (localPart.Length <= 2)
            return localPart + "****" + domain;

        return localPart[..2] + "****" + domain;
    }

    /// <summary>
    /// Masks WeChat ID - keeps first 2 and last 2 characters
    /// Example: wxid_abc123xyz -> wx*******yz
    /// </summary>
    public static string MaskWechat(string wechat)
    {
        if (string.IsNullOrEmpty(wechat))
            return "****";

        if (wechat.Length <= 4)
            return "****";

        var first = wechat[..2];
        var last = wechat[^2..];
        var maskedPart = new string('*', wechat.Length - 4);
        return first + maskedPart + last;
    }
}

/// <summary>
/// Extension method to register the request logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
