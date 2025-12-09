namespace CrmSystem.Api.Models;

/// <summary>
/// 用户实体（可选功能，用于认证）
/// </summary>
public class User
{
    /// <summary>
    /// 用户唯一标识符
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 用户名（唯一，最大 100 字符）
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码哈希（BCrypt 哈希，最大 255 字符）
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户角色（最大 50 字符）
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间（UTC）
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间（UTC）
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// 最后登录时间（UTC）
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }
}
