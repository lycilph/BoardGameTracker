using BoardGameTracker.Application.Authentication.DTO;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Authentication.Commands;

public record class RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<AuthenticationResponse>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator(IValidator<RefreshTokenRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResponse>
{
    private readonly IValidator<RefreshTokenCommand> validator;
    private readonly ITokenService token_service;
    private readonly IIdentityService identity_service;

    public RefreshTokenCommandHandler(IValidator<RefreshTokenCommand> validator, ITokenService token_service, IIdentityService identity_service)
    {
        this.validator = validator;
        this.token_service = token_service;
        this.identity_service = identity_service;
    }

    public async Task<AuthenticationResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return AuthenticationResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));
        
        var request = command.Request;

        var principal = token_service.GetPrincipalFromExpiredToken(request.Token);
        var userid = principal.GetUserId()!;
        var is_persistent = principal.IsPersistent();

        var user = await identity_service.FindUserByIdAsync(userid);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return AuthenticationResponse.Failure("Invalid client request");

        (var token, var refresh_token) = await token_service.GenerateTokensAsync(user, is_persistent);

        user.RefreshToken = refresh_token;
        await identity_service.UpdateUserAsync(user);

        return AuthenticationResponse.Success(token, refresh_token);
    }
}
