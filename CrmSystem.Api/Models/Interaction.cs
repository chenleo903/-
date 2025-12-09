namespace CrmSystem.Api.Models;

/// <summary>
/// 互动记录实体
/// </summary>
public class Interaction
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
    /// 互动发生时间（必填，UTC）
    /// </summary>
    public DateTimeOffset HappenedAt { get; set; }
    
    /// <summary>
    /// 互动渠道（必填）
    /// </summary>
    public InteractionChannel Channel { get; set; }
    
    /// <summary>
    /// 互动时客户所处阶段（可选）
    /// </summary>
    public CustomerStatus? Stage { get; set; }
    
    /// <summary>
    /// 互动标题（必填，最大 200 字符）
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 互动摘要（最大 2000 字符）
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// 原始内容（最大 10000 字符）
    /// </summary>
    public string? RawContent { get; set; }
    
    /// <summary>
    /// 下一步行动（最大 500 字符）
    /// </summary>
    public string? NextAction { get; set; }
    
    /// <summary>
    /// 附件列表（存储为 JSONB）
    /// </summary>
    public List<AttachmentInfo>? Attachments { get; set; }
    
    /// <summary>
    /// 创建时间（UTC）
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间（UTC，并发令牌）
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// 导航属性：关联的客户
    /// </summary>
    public Customer Customer { get; set; } = null!;
}
