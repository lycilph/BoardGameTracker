using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Commands;

public record class LoginCommand(LoginRequest Request) : IRequest<AuthenticationResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IValidator<LoginRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResponse>
{
    private readonly IValidator<LoginCommand> validator;
    private readonly IIdentityService identity_service;
    private readonly ITokenService token_service;

    public LoginCommandHandler(IValidator<LoginCommand> validator, IIdentityService identity_service, ITokenService token_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
        this.token_service = token_service;
    }

    public async Task<AuthenticationResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return AuthenticationResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        var user = await identity_service.FindUserByNameAsync(request.EmailOrUsername) ??
                   await identity_service.FindUserByEmailAsync(request.EmailOrUsername);
        if (user == null || !await identity_service.CheckPasswordAsync(user, request.Password))
            return AuthenticationResponse.Failure("Invalid Email/Username or Password");

        (var token, var refresh_token) = await token_service.GenerateTokensAsync(user, request.RememberMe);

        // This is used when refreshing the access tokens
        user.RefreshToken = refresh_token;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await identity_service.UpdateUserAsync(user);

        return AuthenticationResponse.Success(token, refresh_token);
    }
}