using CrmSystem.Api.Models;

namespace CrmSystem.Api.Services;

/// <summary>
/// 认证业务逻辑接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 登录验证并生成 JWT 令牌
    /// </summary>
    Task<(string Token, DateTimeOffset ExpiresAt)> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建初始管理员用户
    /// </summary>
    Task<User> CreateInitialAdminAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查是否存在任何用户
    /// </summary>
    Task<bool> HasAnyUserAsync(CancellationToken cancellationToken = default);
}
