using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Game.DTO;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Application.Game.Commands;

public record class UpdateCollectionCommand(UpdateCollectionRequest Request) : IRequest<GameResponse>;

public class UpdateCollectionCommandValidator : AbstractValidator<UpdateCollectionCommand>
{
    public UpdateCollectionCommandValidator(IValidator<UpdateCollectionRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class UpdateCollectionCommandHandler : IRequestHandler<UpdateCollectionCommand, GameResponse>
{
    private readonly IValidator<UpdateCollectionCommand> validator;
    private readonly IRepository<ProfileDTO> profile_store;
    private readonly IRepository<BoardGameDTO> game_store;
    private readonly ILogger<UpdateCollectionCommandHandler> logger;

    public UpdateCollectionCommandHandler(IValidator<UpdateCollectionCommand> validator, IRepository<ProfileDTO> profile_store, IRepository<BoardGameDTO> game_store, ILogger<UpdateCollectionCommandHandler> logger)
    {
        this.validator = validator;
        this.profile_store = profile_store;
        this.game_store = game_store;
        this.logger = logger;
    }

    public async Task<GameResponse> Handle(UpdateCollectionCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return GameResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        ProfileDTO profile;
        if (await profile_store.ExistsAsync(request.UserId, cancellationToken: cancellationToken))
            profile = await profile_store.GetAsync(request.UserId, cancellationToken: cancellationToken);
        else
            profile = new() { Id = request.UserId };

        // Update the profile
        profile.BoardGameIds = request.Games.Select(x => x.Id).ToList();
        await profile_store.UpdateAsync(profile, cancellationToken);

        // Update the games
        await game_store.UpdateAsync(request.Games, cancellationToken);

        return GameResponse.Success();
    }
}