using FluentValidation;

namespace CrmSystem.Api.DTOs.Validators;

/// <summary>
/// CreateInteractionRequest 验证器
/// </summary>
public class CreateInteractionRequestValidator : AbstractValidator<CreateInteractionRequest>
{
    public CreateInteractionRequestValidator()
    {
        // 互动发生时间验证
        RuleFor(x => x.HappenedAt)
            .NotEmpty().WithMessage("Happened at is required");
        
        // 互动渠道验证
        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("Invalid interaction channel");
        
        // 互动阶段验证（可选）
        RuleFor(x => x.Stage)
            .IsInEnum().When(x => x.Stage.HasValue)
            .WithMessage("Invalid customer status");
        
        // 互动标题验证
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        
        // 互动摘要验证
        RuleFor(x => x.Summary)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Summary))
            .WithMessage("Summary must not exceed 2000 characters");
        
        // 原始内容验证
        RuleFor(x => x.RawContent)
            .MaximumLength(10000).When(x => !string.IsNullOrEmpty(x.RawContent))
            .WithMessage("Raw content must not exceed 10000 characters");
        
        // 下一步行动验证
        RuleFor(x => x.NextAction)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.NextAction))
            .WithMessage("Next action must not exceed 500 characters");
        
        // 附件验证
        RuleFor(x => x.Attachments)
            .Must(attachments => attachments == null || attachments.All(a => 
                !string.IsNullOrEmpty(a.Url) && a.Url.Length <= 500))
            .WithMessage("Each attachment URL must not be empty and must not exceed 500 characters");
        
        RuleFor(x => x.Attachments)
            .Must(attachments => attachments == null || attachments.All(a => 
                a.FileName == null || a.FileName.Length <= 255))
            .WithMessage("Each attachment file name must not exceed 255 characters");
    }
}
