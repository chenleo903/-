namespace CrmSystem.Api.Middleware;

/// <summary>
/// CORS 配置扩展方法
/// </summary>
public static class CorsConfiguration
{
    public const string CorsPolicy = "CrmCorsPolicy";

    /// <summary>
    /// 配置 CORS 服务
    /// </summary>
    public static IServiceCollection AddCrmCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "http://localhost:5173" };

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("ETag", "Location")
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// 使用 CORS 中间件
    /// </summary>
    public static IApplicationBuilder UseCrmCors(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicy);
    }
}
