using FluentValidation;

namespace BoardGameTracker.Application.Identity.DTO;

public class UpdateBGGUsernameRequest
{
    public string UserId { get; set; } = string.Empty;
    public string BGGUsername { get; set; } = string.Empty;
}

public class UpdateBGGUsernameRequestValidator : AbstractValidator<UpdateBGGUsernameRequest>
{
    public UpdateBGGUsernameRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.BGGUsername).NotEmpty().WithMessage("BGG username is required");
    }
}