namespace CrmSystem.Api.Models;

/// <summary>
/// 客户实体
/// </summary>
public class Customer
{
    /// <summary>
    /// 客户唯一标识符
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 公司名称（必填，最大 200 字符）
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// 联系人姓名（必填，最大 200 字符）
    /// </summary>
    public string ContactName { get; set; } = string.Empty;
    
    /// <summary>
    /// 微信号（最大 100 字符）
    /// </summary>
    public string? Wechat { get; set; }
    
    /// <summary>
    /// 电话号码（最大 50 字符）
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// 邮箱地址（最大 255 字符）
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 行业（最大 100 字符）
    /// </summary>
    public string? Industry { get; set; }
    
    /// <summary>
    /// 客户来源
    /// </summary>
    public CustomerSource? Source { get; set; }
    
    /// <summary>
    /// 客户状态
    /// </summary>
    public CustomerStatus Status { get; set; }
    
    /// <summary>
    /// 标签数组（PostgreSQL text[]，每个标签最大 50 字符）
    /// </summary>
    public string[]? Tags { get; set; }
    
    /// <summary>
    /// 客户评分（0-100）
    /// </summary>
    public int Score { get; set; }
    
    /// <summary>
    /// 最后互动时间（UTC）
    /// </summary>
    public DateTimeOffset? LastInteractionAt { get; set; }
    
    /// <summary>
    /// 创建时间（UTC）
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间（UTC，并发令牌）
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// 软删除标志
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// 导航属性：关联的互动记录
    /// </summary>
    public ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}
