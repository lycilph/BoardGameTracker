using FluentValidation;

namespace BoardGameTracker.Application.Authentication.DTO;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BGGUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).MinimumLength(5).WithMessage("Username is required and must be at least 5 characters");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Valid email is required");
        RuleFor(x => x.Password).Length(6, 20).WithMessage("The password must be between 6 and 20 characters long");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("The password and confirmation password do not match");
    }
}