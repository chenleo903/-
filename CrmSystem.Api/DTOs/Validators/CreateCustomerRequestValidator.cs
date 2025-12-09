using FluentValidation;

namespace CrmSystem.Api.DTOs.Validators;

/// <summary>
/// CreateCustomerRequest 验证器
/// </summary>
public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        // 公司名称验证
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters");
        
        // 联系人姓名验证
        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Contact name is required")
            .MaximumLength(200).WithMessage("Contact name must not exceed 200 characters");
        
        // 邮箱验证（仅在非空时验证格式）
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format")
            .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email must not exceed 255 characters");
        
        // 电话验证
        RuleFor(x => x.Phone)
            .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone must not exceed 50 characters");
        
        // 微信号验证
        RuleFor(x => x.Wechat)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Wechat))
            .WithMessage("Wechat must not exceed 100 characters");
        
        // 行业验证
        RuleFor(x => x.Industry)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Industry))
            .WithMessage("Industry must not exceed 100 characters");
        
        // 评分验证
        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100)
            .WithMessage("Score must be between 0 and 100");
        
        // 标签验证（每个标签最大 50 字符）
        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.All(t => t.Length <= 50))
            .WithMessage("Each tag must not exceed 50 characters");
    }
}
