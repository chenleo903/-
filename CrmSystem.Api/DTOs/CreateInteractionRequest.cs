using System.ComponentModel.DataAnnotations;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.DTOs;

/// <summary>
/// 创建互动记录请求 DTO
/// </summary>
public class CreateInteractionRequest
{
    /// <summary>
    /// 互动发生时间（必填）
    /// </summary>
    [Required(ErrorMessage = "Happened at is required")]
    public DateTimeOffset HappenedAt { get; set; }
    
    /// <summary>
    /// 互动渠道（必填）
    /// </summary>
    [Required(ErrorMessage = "Channel is required")]
    [EnumDataType(typeof(InteractionChannel), ErrorMessage = "Invalid interaction channel")]
    public InteractionChannel Channel { get; set; }
    
    /// <summary>
    /// 互动时客户所处阶段（可选）
    /// </summary>
    [EnumDataType(typeof(CustomerStatus), ErrorMessage = "Invalid customer status")]
    public CustomerStatus? Stage { get; set; }
    
    /// <summary>
    /// 互动标题（必填，最大 200 字符）
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title must not exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 互动摘要（最大 2000 字符）
    /// </summary>
    [MaxLength(2000, ErrorMessage = "Summary must not exceed 2000 characters")]
    public string? Summary { get; set; }
    
    /// <summary>
    /// 原始内容（最大 10000 字符）
    /// </summary>
    [MaxLength(10000, ErrorMessage = "Raw content must not exceed 10000 characters")]
    public string? RawContent { get; set; }
    
    /// <summary>
    /// 下一步行动（最大 500 字符）
    /// </summary>
    [MaxLength(500, ErrorMessage = "Next action must not exceed 500 characters")]
    public string? NextAction { get; set; }
    
    /// <summary>
    /// 附件列表
    /// </summary>
    public List<AttachmentInfo>? Attachments { get; set; }
}
