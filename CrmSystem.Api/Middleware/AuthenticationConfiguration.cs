using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CrmSystem.Api.Middleware;

/// <summary>
/// 认证配置
/// </summary>
public class AuthSettings
{
    public bool EnableAuth { get; set; }
    public string? AdminUsername { get; set; }
    public string? AdminPassword { get; set; }
    public string? JwtSecret { get; set; }
    public int JwtExpiryMinutes { get; set; } = 60;

    /// <summary>
    /// 验证配置
    /// </summary>
    public void Validate()
    {
        if (EnableAuth)
        {
            if (string.IsNullOrWhiteSpace(JwtSecret))
                throw new InvalidOperationException("JWT_SECRET is required when ENABLE_AUTH=true");

            if (string.IsNullOrWhiteSpace(AdminUsername) || string.IsNullOrWhiteSpace(AdminPassword))
                throw new InvalidOperationException("ADMIN_USERNAME and ADMIN_PASSWORD are required when ENABLE_AUTH=true");
        }
    }
}

/// <summary>
/// 认证配置扩展方法
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// 配置 JWT 认证服务
    /// </summary>
    public static IServiceCollection AddCrmAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration,
        out AuthSettings authSettings)
    {
        var settings = configuration.GetSection("Auth").Get<AuthSettings>() ?? new AuthSettings();
        settings.Validate();
        authSettings = settings;

        if (settings.EnableAuth)
        {
            var jwtSecret = settings.JwtSecret!;
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
        }

        // Register AuthSettings as a singleton for injection
        services.AddSingleton(settings);

        return services;
    }

    /// <summary>
    /// 使用认证中间件（仅在启用认证时）
    /// </summary>
    public static IApplicationBuilder UseCrmAuthentication(
        this IApplicationBuilder app, 
        AuthSettings authSettings)
    {
        if (authSettings.EnableAuth)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        return app;
    }
}
