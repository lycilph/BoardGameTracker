using FluentValidation;

namespace BoardGameTracker.Application.Authentication.DTO;

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
    }
}