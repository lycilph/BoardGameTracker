using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Application.Identity.Commands;

public record class DeleteAccountCommand(string UserId) : IRequest<DeleteAccountResponse>;

public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().WithMessage("UserId is required");
    }
}

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, DeleteAccountResponse>
{
    private readonly IValidator<DeleteAccountCommand> validator;
    private readonly IIdentityService identity_service;
    private readonly IRepository<ProfileDTO> profile_store;
    private readonly ILogger<DeleteAccountCommandHandler> logger;

    public DeleteAccountCommandHandler(IValidator<DeleteAccountCommand> validator, IIdentityService identity_service, IRepository<ProfileDTO> profile_store, ILogger<DeleteAccountCommandHandler> logger)
    {
        this.validator = validator;
        this.identity_service = identity_service;
        this.profile_store = profile_store;
        this.logger = logger;
    }

    public async Task<DeleteAccountResponse> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return DeleteAccountResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var user = await identity_service.FindUserByIdAsync(command.UserId);
        if (user == null)
            return DeleteAccountResponse.Failure("Unknown user");

        var response = await identity_service.DeleteUserAsync(user);
        if (!response.Succeeded)
            return DeleteAccountResponse.Failure(response.Errors.Select(e => e.Description));

        // This should be cleaned up and move to a GameService class (analogous to the IdentityService
        try
        {
            await profile_store.DeleteAsync(user.Id, cancellationToken: cancellationToken);
        }
        catch (CosmosException ce)
        {
            logger.LogError(ce, "Couldn't delete profile for user {user}", user.UserName);
        }

        return DeleteAccountResponse.Success();
    }
}