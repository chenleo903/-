using CrmSystem.Api.Models;

namespace CrmSystem.Api.Repositories;

/// <summary>
/// 用户数据访问接口
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查是否存在任何用户
    /// </summary>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 创建用户
    /// </summary>
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新用户最后登录时间
    /// </summary>
    Task UpdateLastLoginAsync(Guid userId, DateTimeOffset lastLoginAt, CancellationToken cancellationToken = default);
}
