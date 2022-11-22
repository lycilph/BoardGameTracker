using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Commands;

public record class UpdateAccountCommand(UpdateAccountRequest Request) : IRequest<UpdateAccountResponse>;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator(IValidator<UpdateAccountRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
{
    private readonly IValidator<UpdateAccountCommand> validator;
    private readonly IIdentityService identity_service;

    public UpdateAccountCommandHandler(IValidator<UpdateAccountCommand> validator, IIdentityService identity_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
    }

    public async Task<UpdateAccountResponse> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return UpdateAccountResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        var user = await identity_service.FindUserByIdAsync(request.UserId);
        if (user == null)
            return UpdateAccountResponse.Failure("Unknown user");

        // Check if this is a no-op
        if (identity_service.NormalizeUsername(request.Username) == user.NormalizedUserName &&
           request.BGGUsername == user.BGGUsername)
            return UpdateAccountResponse.NoOp();

        // Check if the new username is already taken
        var temp = await identity_service.FindUserByNameAsync(request.Username);
        if (temp != null && temp.Id != user.Id)
            return UpdateAccountResponse.Failure("Username already taken");

        user.UserName = request.Username;
        user.BGGUsername = request.BGGUsername;
        var response = await identity_service.UpdateUserAsync(user);
        if (!response.Succeeded)
            return UpdateAccountResponse.Failure(response.Errors.Select(e => e.Description));

        return UpdateAccountResponse.Success();
    }
}