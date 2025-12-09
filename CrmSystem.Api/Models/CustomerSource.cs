namespace CrmSystem.Api.Models;

/// <summary>
/// 客户来源枚举
/// </summary>
public enum CustomerSource
{
    /// <summary>
    /// 网站
    /// </summary>
    Website,
    
    /// <summary>
    /// 推荐
    /// </summary>
    Referral,
    
    /// <summary>
    /// 社交媒体
    /// </summary>
    SocialMedia,
    
    /// <summary>
    /// 活动
    /// </summary>
    Event,
    
    /// <summary>
    /// 直接联系
    /// </summary>
    DirectContact,
    
    /// <summary>
    /// 其他
    /// </summary>
    Other
}
