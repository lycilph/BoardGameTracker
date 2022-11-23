using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Services.Mail;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Commands;

public record class UpdateEmailCommand(UpdateEmailRequest Request, string Origin) : IRequest<UpdateEmailResponse>;

public class UpdateEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
{
    public UpdateEmailCommandValidator(IValidator<UpdateEmailRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, UpdateEmailResponse>
{
    private readonly IValidator<UpdateEmailCommand> validator;
    private readonly IIdentityService identity_service;
    private readonly IMailService mail_service;

    public UpdateEmailCommandHandler(IValidator<UpdateEmailCommand> validator, IIdentityService identity_service, IMailService mail_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
        this.mail_service = mail_service;
    }

    public async Task<UpdateEmailResponse> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return UpdateEmailResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        var user = await identity_service.FindUserByIdAsync(request.UserId);
        if (user == null)
            return UpdateEmailResponse.Failure("Unknown user");

        // Check if this is a no-op
        if (identity_service.NormalizeUsername(request.NewEmail) == user.NormalizedEmail)
            return UpdateEmailResponse.NoOp();

        // Check if the new username is already taken
        var temp = await identity_service.FindUserByEmailAsync(request.NewEmail);
        if (temp != null && temp.Id != user.Id)
            return UpdateEmailResponse.Failure("Email already taken");

        user.Email = request.NewEmail;
        user.EmailConfirmed = false;
        var response = await identity_service.UpdateUserAsync(user);
        if (!response.Succeeded)
            return UpdateEmailResponse.Failure(response.Errors.Select(e => e.Description));

        await mail_service.SendConfirmationEmail(user, command.Origin);

        return UpdateEmailResponse.Success();
    }
}