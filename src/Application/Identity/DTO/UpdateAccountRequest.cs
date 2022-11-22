using FluentValidation;

namespace BoardGameTracker.Application.Identity.DTO;

public class UpdateAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string BGGUsername { get; set; } = string.Empty;
}

public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
    }
}