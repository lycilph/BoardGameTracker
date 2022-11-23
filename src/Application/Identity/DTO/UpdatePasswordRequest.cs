using FluentValidation;

namespace BoardGameTracker.Application.Identity.DTO;

public class UpdatePasswordRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
{
    public UpdatePasswordRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.NewPassword).NotEqual(x => x.Password).WithMessage("The new password must be different than the current password");
        RuleFor(x => x.NewPassword).Length(6, 20).WithMessage("The new password must be between 6 and 20 characters long");
        RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match");
    }
}