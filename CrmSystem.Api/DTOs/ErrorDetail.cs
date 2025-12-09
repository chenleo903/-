namespace CrmSystem.Api.DTOs;

/// <summary>
/// 错误详情
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// 错误字段（可选，仅验证错误时提供）
    /// </summary>
    public string? Field { get; set; }
    
    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// 当前值（可选，用于并发冲突时返回当前版本）
    /// </summary>
    public string? CurrentValue { get; set; }
}
