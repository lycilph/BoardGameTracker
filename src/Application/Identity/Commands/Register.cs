using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Commands;

public record class RegisterCommand(RegisterRequest Request) : IRequest<AuthenticationResponse>;

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

    public RegisterUserCommandHandler(IValidator<RegisterCommand> validator, IIdentityService identity_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
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

        return AuthenticationResponse.Success();
    }
}