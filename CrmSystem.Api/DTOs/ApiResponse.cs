namespace CrmSystem.Api.DTOs;

/// <summary>
/// 统一 API 响应格式
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 请求是否成功
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// 错误信息列表
    /// </summary>
    public List<ErrorDetail> Errors { get; set; } = new();
}
