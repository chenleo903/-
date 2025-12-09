using CrmSystem.Api.Models;
using CrmSystem.Api.DTOs;

namespace CrmSystem.Api.Repositories;

/// <summary>
/// 客户数据访问接口
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// 根据 ID 获取客户（包含未删除的客户）
    /// </summary>
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据 ID 获取客户（包含已删除的客户，用于内部验证）
    /// </summary>
    Task<Customer?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查客户是否存在（根据公司名称和联系人姓名）
    /// </summary>
    Task<bool> ExistsAsync(string companyName, string contactName, Guid? excludeId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 搜索和筛选客户列表（分页）
    /// </summary>
    Task<PagedResponse<Customer>> SearchAsync(CustomerSearchRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 创建客户
    /// </summary>
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新客户
    /// </summary>
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 软删除客户
    /// </summary>
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
