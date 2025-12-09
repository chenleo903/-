using CrmSystem.Api.Models;

namespace CrmSystem.Api.DTOs;

/// <summary>
/// 客户响应 DTO
/// </summary>
public class CustomerResponse
{
    /// <summary>
    /// 客户唯一标识符
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string ContactName { get; set; } = string.Empty;
    
    /// <summary>
    /// 微信号
    /// </summary>
    public string? Wechat { get; set; }
    
    /// <summary>
    /// 电话号码
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 行业
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
    /// 标签数组
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
    /// 更新时间（UTC）
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
