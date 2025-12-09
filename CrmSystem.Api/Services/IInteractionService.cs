using CrmSystem.Api.DTOs;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.Services;

/// <summary>
/// 互动记录业务逻辑接口
/// </summary>
public interface IInteractionService
{
    /// <summary>
    /// 创建互动记录（含事务更新 LastInteractionAt）
    /// </summary>
    Task<Interaction> CreateInteractionAsync(Guid customerId, CreateInteractionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据客户 ID 获取互动记录列表（按时间降序）
    /// </summary>
    Task<List<Interaction>> GetInteractionsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取互动记录
    /// </summary>
    Task<Interaction> GetInteractionByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新互动记录（含并发控制）
    /// </summary>
    Task<Interaction> UpdateInteractionAsync(Guid id, UpdateInteractionRequest request, DateTimeOffset? originalUpdatedAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除互动记录（含事务重新计算 LastInteractionAt）
    /// </summary>
    Task DeleteInteractionAsync(Guid id, DateTimeOffset? originalUpdatedAt = null, CancellationToken cancellationToken = default);
}
