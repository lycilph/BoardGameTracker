using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Authentication.DTO;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Services.Mail;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Authentication.Commands;

public record class RegisterCommand(RegisterRequest Request, string Origin) : IRequest<AuthenticationResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IValidator<RegisterRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResponse>
{
    private readonly IValidator<RegisterCommand> validator;
    private readonly IIdentityService identity_service;
    private readonly IMailService mail_service;

    public RegisterUserCommandHandler(IValidator<RegisterCommand> validator, IIdentityService identity_service, IMailService mail_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
        this.mail_service = mail_service;
    }

    public async Task<AuthenticationResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Validation of request is done in command validator
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return AuthenticationResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        // Check if username is already in use
        var user_from_name = await identity_service.FindUserByNameAsync(request.Username);
        if (user_from_name != null)
            return AuthenticationResponse.Failure($"Username {request.Username} is already in use");

        // Check if email is already in use
        var user_from_email = await identity_service.FindUserByEmailAsync(request.Email);
        if (user_from_email != null)
            return AuthenticationResponse.Failure($"Email {request.Email} is already in use");

        // Add user
        var user = new ApplicationUser { UserName = request.Username, Email = request.Email };
        var create_result = await identity_service.CreateUserAsync(user, request.Password);
        if (!create_result.Succeeded)
            return AuthenticationResponse.Failure(create_result.Errors.Select(e => e.Description));

        // Add role
        var role_result = await identity_service.AddUserToRoleAsync(user, RoleConstants.UserRole);
        if (!role_result.Succeeded)
            return AuthenticationResponse.Failure(create_result.Errors.Select(e => e.Description));

        // BGG username
        user.BGGUsername = request.BGGUsername;
        var bgg_username_result = await identity_service.UpdateUserAsync(user);
        if (!bgg_username_result.Succeeded)
            return AuthenticationResponse.Failure(create_result.Errors.Select(e => e.Description));

        await mail_service.SendConfirmationEmail(user, command.Origin);

        return AuthenticationResponse.Success();
    }
}