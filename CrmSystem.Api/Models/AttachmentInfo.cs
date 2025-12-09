namespace CrmSystem.Api.Models;

/// <summary>
/// 附件信息类（存储为 JSONB）
/// </summary>
public class AttachmentInfo
{
    /// <summary>
    /// 附件 URL（最大 500 字符）
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// 文件名（最大 255 字符）
    /// </summary>
    public string? FileName { get; set; }
    
    /// <summary>
    /// 文件大小（字节数）
    /// </summary>
    public long? FileSize { get; set; }
}
