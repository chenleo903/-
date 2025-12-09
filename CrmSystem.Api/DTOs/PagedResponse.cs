namespace CrmSystem.Api.DTOs;

/// <summary>
/// 分页响应 DTO
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// 数据项列表
    /// </summary>
    public List<T> Items { get; set; } = new();
    
    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }
}
