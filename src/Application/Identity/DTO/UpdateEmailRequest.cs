using FluentValidation;

namespace BoardGameTracker.Application.Identity.DTO;

public class UpdateEmailRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
}

public class UpdateEmailRequestValidator : AbstractValidator<UpdateEmailRequest>
{
    public UpdateEmailRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email is required");
        RuleFor(x => x.NewEmail).EmailAddress().WithMessage("New Email is required");
        RuleFor(x => x.NewEmail).NotEqual(x => x.Email).WithMessage("New Email should be diffrent from current email");
    }
}