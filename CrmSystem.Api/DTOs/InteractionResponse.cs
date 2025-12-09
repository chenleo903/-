using CrmSystem.Api.Models;

namespace CrmSystem.Api.DTOs;

/// <summary>
/// 互动记录响应 DTO
/// </summary>
public class InteractionResponse
{
    /// <summary>
    /// 互动记录唯一标识符
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 关联的客户 ID
    /// </summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>
    /// 互动发生时间（UTC）
    /// </summary>
    public DateTimeOffset HappenedAt { get; set; }
    
    /// <summary>
    /// 互动渠道
    /// </summary>
    public InteractionChannel Channel { get; set; }
    
    /// <summary>
    /// 互动时客户所处阶段
    /// </summary>
    public CustomerStatus? Stage { get; set; }
    
    /// <summary>
    /// 互动标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 互动摘要
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// 原始内容
    /// </summary>
    public string? RawContent { get; set; }
    
    /// <summary>
    /// 下一步行动
    /// </summary>
    public string? NextAction { get; set; }
    
    /// <summary>
    /// 附件列表
    /// </summary>
    public List<AttachmentInfo>? Attachments { get; set; }
    
    /// <summary>
    /// 创建时间（UTC）
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间（UTC）
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
