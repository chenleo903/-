using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CrmSystem.Api.Exceptions;
using CrmSystem.Api.Models;
using CrmSystem.Api.Repositories;

namespace CrmSystem.Api.Services;

/// <summary>
/// 认证业务逻辑实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(string Token, DateTimeOffset ExpiresAt)> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        // 查找用户
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent username: {Username}", username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        // 验证密码
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Login attempt with incorrect password for user: {Username}", username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        // 更新最后登录时间
        await _userRepository.UpdateLastLoginAsync(user.Id, DateTimeOffset.UtcNow, cancellationToken);

        // 生成 JWT 令牌
        var jwtSecret = _configuration["JWT_SECRET"] ?? _configuration["Auth:JwtSecret"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new InvalidOperationException("JWT_SECRET is not configured");
        }

        var jwtExpiryMinutes = int.TryParse(
            _configuration["JWT_EXPIRY_MINUTES"] ?? _configuration["Auth:JwtExpiryMinutes"],
            out var minutes) ? minutes : 60;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(jwtExpiryMinutes);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogInformation("User {Username} logged in successfully", username);

        return (tokenString, expiresAt);
    }

    public async Task<User> CreateInitialAdminAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        // 检查是否已存在用户
        var hasUsers = await _userRepository.AnyAsync(cancellationToken);
        if (hasUsers)
        {
            throw new InvalidOperationException("Users already exist in the system");
        }

        // 哈希密码
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        var admin = new User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            PasswordHash = passwordHash,
            Role = "Admin",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = null
        };

        await _userRepository.CreateAsync(admin, cancellationToken);

        _logger.LogInformation("Initial admin user created: {Username}", username);

        return admin;
    }

    public async Task<bool> HasAnyUserAsync(CancellationToken cancellationToken = default)
    {
        return await _userRepository.AnyAsync(cancellationToken);
    }
}
