using System.ComponentModel.DataAnnotations;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.DTOs;

/// <summary>
/// 客户搜索请求 DTO
/// </summary>
public class CustomerSearchRequest
{
    /// <summary>
    /// 关键词搜索（搜索公司名称和联系人姓名）
    /// </summary>
    public string? Keyword { get; set; }
    
    /// <summary>
    /// 状态筛选
    /// </summary>
    [EnumDataType(typeof(CustomerStatus), ErrorMessage = "Invalid customer status")]
    public CustomerStatus? Status { get; set; }
    
    /// <summary>
    /// 行业筛选
    /// </summary>
    public string? Industry { get; set; }
    
    /// <summary>
    /// 来源筛选
    /// </summary>
    [EnumDataType(typeof(CustomerSource), ErrorMessage = "Invalid customer source")]
    public CustomerSource? Source { get; set; }
    
    /// <summary>
    /// 排序字段（CreatedAt, UpdatedAt, LastInteractionAt）
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// 排序方向（asc, desc）
    /// </summary>
    public string? SortOrder { get; set; }
    
    /// <summary>
    /// 页码（默认为 1）
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int? Page { get; set; } = 1;
    
    /// <summary>
    /// 每页大小（默认为 20，最大为 100）
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int? PageSize { get; set; } = 20;
}
