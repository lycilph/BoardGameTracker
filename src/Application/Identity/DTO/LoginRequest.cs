using FluentValidation;

namespace BoardGameTracker.Application.Identity.DTO;

public class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailOrUsername).NotEmpty().WithMessage("Email/Username is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}