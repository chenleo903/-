using CrmSystem.Api.Models;

namespace CrmSystem.Api.Repositories;

/// <summary>
/// 互动记录数据访问接口
/// </summary>
public interface IInteractionRepository
{
    /// <summary>
    /// 根据 ID 获取互动记录
    /// </summary>
    Task<Interaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据客户 ID 获取互动记录列表（按时间降序）
    /// </summary>
    Task<List<Interaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取客户最新的互动记录
    /// </summary>
    Task<Interaction?> GetLatestByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 创建互动记录
    /// </summary>
    Task<Interaction> CreateAsync(Interaction interaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新互动记录
    /// </summary>
    Task<Interaction> UpdateAsync(Interaction interaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除互动记录（物理删除）
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
