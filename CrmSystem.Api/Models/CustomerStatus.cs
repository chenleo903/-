namespace CrmSystem.Api.Models;

/// <summary>
/// 客户状态枚举
/// </summary>
public enum CustomerStatus
{
    /// <summary>
    /// 潜在客户
    /// </summary>
    Lead,
    
    /// <summary>
    /// 已联系
    /// </summary>
    Contacted,
    
    /// <summary>
    /// 需求已分析
    /// </summary>
    NeedsAnalyzed,
    
    /// <summary>
    /// 已报价
    /// </summary>
    Quoted,
    
    /// <summary>
    /// 谈判中
    /// </summary>
    Negotiating,
    
    /// <summary>
    /// 已成交
    /// </summary>
    Won,
    
    /// <summary>
    /// 已失败
    /// </summary>
    Lost
}
