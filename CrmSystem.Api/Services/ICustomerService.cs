using CrmSystem.Api.DTOs;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.Services;

/// <summary>
/// 客户业务逻辑接口
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// 创建客户（含唯一性验证）
    /// </summary>
    Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取客户详情
    /// </summary>
    Task<Customer> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新客户（含并发控制）
    /// </summary>
    Task<Customer> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, DateTimeOffset? originalUpdatedAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 软删除客户
    /// </summary>
    Task DeleteCustomerAsync(Guid id, DateTimeOffset? originalUpdatedAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索和筛选客户列表
    /// </summary>
    Task<PagedResponse<Customer>> SearchCustomersAsync(CustomerSearchRequest request, CancellationToken cancellationToken = default);
}
