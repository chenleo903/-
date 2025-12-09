using FluentValidation;

namespace CrmSystem.Api.DTOs.Validators;

/// <summary>
/// CustomerSearchRequest 验证器
/// </summary>
public class CustomerSearchRequestValidator : AbstractValidator<CustomerSearchRequest>
{
    public CustomerSearchRequestValidator()
    {
        // 状态验证
        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue)
            .WithMessage("Invalid customer status");
        
        // 来源验证
        RuleFor(x => x.Source)
            .IsInEnum().When(x => x.Source.HasValue)
            .WithMessage("Invalid customer source");
        
        // 排序字段验证
        RuleFor(x => x.SortBy)
            .Must(sortBy => sortBy == null || 
                new[] { "CreatedAt", "UpdatedAt", "LastInteractionAt" }.Contains(sortBy))
            .WithMessage("Sort by must be one of: CreatedAt, UpdatedAt, LastInteractionAt");
        
        // 排序方向验证
        RuleFor(x => x.SortOrder)
            .Must(sortOrder => sortOrder == null || 
                new[] { "asc", "desc" }.Contains(sortOrder.ToLower()))
            .WithMessage("Sort order must be either 'asc' or 'desc'");
        
        // 页码验证
        RuleFor(x => x.Page)
            .GreaterThan(0).When(x => x.Page.HasValue)
            .WithMessage("Page must be greater than 0");
        
        // 每页大小验证
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).When(x => x.PageSize.HasValue)
            .WithMessage("Page size must be between 1 and 100");
    }
}
