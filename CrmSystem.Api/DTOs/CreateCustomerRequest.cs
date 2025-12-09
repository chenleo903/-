using System.ComponentModel.DataAnnotations;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.DTOs;

/// <summary>
/// 创建客户请求 DTO
/// </summary>
public class CreateCustomerRequest
{
    /// <summary>
    /// 公司名称（必填，最大 200 字符）
    /// </summary>
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(200, ErrorMessage = "Company name must not exceed 200 characters")]
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// 联系人姓名（必填，最大 200 字符）
    /// </summary>
    [Required(ErrorMessage = "Contact name is required")]
    [MaxLength(200, ErrorMessage = "Contact name must not exceed 200 characters")]
    public string ContactName { get; set; } = string.Empty;
    
    /// <summary>
    /// 微信号（最大 100 字符）
    /// </summary>
    [MaxLength(100, ErrorMessage = "Wechat must not exceed 100 characters")]
    public string? Wechat { get; set; }
    
    /// <summary>
    /// 电话号码（最大 50 字符）
    /// </summary>
    [MaxLength(50, ErrorMessage = "Phone must not exceed 50 characters")]
    public string? Phone { get; set; }
    
    /// <summary>
    /// 邮箱地址（最大 255 字符）
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email must not exceed 255 characters")]
    public string? Email { get; set; }
    
    /// <summary>
    /// 行业（最大 100 字符）
    /// </summary>
    [MaxLength(100, ErrorMessage = "Industry must not exceed 100 characters")]
    public string? Industry { get; set; }
    
    /// <summary>
    /// 客户来源
    /// </summary>
    [EnumDataType(typeof(CustomerSource), ErrorMessage = "Invalid customer source")]
    public CustomerSource? Source { get; set; }
    
    /// <summary>
    /// 客户状态（默认为 Lead）
    /// </summary>
    [EnumDataType(typeof(CustomerStatus), ErrorMessage = "Invalid customer status")]
    public CustomerStatus Status { get; set; } = CustomerStatus.Lead;
    
    /// <summary>
    /// 标签数组（每个标签最大 50 字符）
    /// </summary>
    public string[]? Tags { get; set; }
    
    /// <summary>
    /// 客户评分（0-100，默认为 0）
    /// </summary>
    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
    public int Score { get; set; } = 0;
}
