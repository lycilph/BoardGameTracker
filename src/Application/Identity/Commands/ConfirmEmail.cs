using Azure.Core;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BoardGameTracker.Application.Identity.Commands;

public record class ConfirmEmailCommand(string UserId, string Code) : IRequest<AuthenticationResponse>;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().WithMessage("UserId is required");
        RuleFor(x => x.Code).NotNull().WithMessage("Code is required");
    }
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, AuthenticationResponse>
{
    private readonly IValidator<ConfirmEmailCommand> validator;
    private readonly IIdentityService identity_service;

    public ConfirmEmailCommandHandler(IValidator<ConfirmEmailCommand> validator, IIdentityService identity_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
    }

    public async Task<AuthenticationResponse> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return AuthenticationResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var user = await identity_service.FindUserByIdAsync(command.UserId);
        if (user == null)
            return AuthenticationResponse.Failure("Invalid user id");

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.Code));
        var result = await identity_service.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return AuthenticationResponse.Success();
        else
            return AuthenticationResponse.Failure($"An error occurred while confirming {user.Email}");
    }
}
