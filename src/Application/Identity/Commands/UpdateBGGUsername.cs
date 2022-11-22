using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Commands;

public record class UpdateBGGUsernameCommand(UpdateBGGUsernameRequest Request) : IRequest<UpdateBGGUsernameResponse>;

public class UpdateBGGUsernameCommandValidator : AbstractValidator<UpdateBGGUsernameCommand>
{
    public UpdateBGGUsernameCommandValidator(IValidator<UpdateBGGUsernameRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class UpdateBGGUsernameCommandHandler : IRequestHandler<UpdateBGGUsernameCommand, UpdateBGGUsernameResponse>
{
    private readonly IValidator<UpdateBGGUsernameCommand> validator;
    private readonly IIdentityService identity_service;

    public UpdateBGGUsernameCommandHandler(IValidator<UpdateBGGUsernameCommand> validator, IIdentityService identity_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
    }

    public async Task<UpdateBGGUsernameResponse> Handle(UpdateBGGUsernameCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return UpdateBGGUsernameResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        var user = await identity_service.FindUserByIdAsync(request.UserId);
        if (user == null)
            return UpdateBGGUsernameResponse.Failure("Unknown user");

        if (user.BGGUsername == request.BGGUsername)
            return UpdateBGGUsernameResponse.NoOp();

        user.BGGUsername = request.BGGUsername;
        var response = await identity_service.UpdateUserAsync(user);
        if (!response.Succeeded)
            return UpdateBGGUsernameResponse.Failure(response.Errors.Select(e => e.Description));

        return UpdateBGGUsernameResponse.Success();
    }
}