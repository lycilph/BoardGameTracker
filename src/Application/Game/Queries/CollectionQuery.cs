using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Game.DTO;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Application.Game.Queries;

public record class CollectionQuery(string UserId) : IRequest<GameResponse>;

public class CollectionQueryValidator : AbstractValidator<CollectionQuery>
{
    public CollectionQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}

public class CollectionQueryHandler : IRequestHandler<CollectionQuery, GameResponse>
{
    private readonly IRepository<ProfileDTO> profile_store;
    private readonly IRepository<BoardGameDTO> game_store;
    private readonly IValidator<CollectionQuery> validator;
    private readonly ILogger<CollectionQueryHandler> logger;

    public CollectionQueryHandler(IRepository<ProfileDTO> profile_store, IRepository<BoardGameDTO> game_store, IValidator<CollectionQuery> validator, ILogger<CollectionQueryHandler> logger)
    {
        this.profile_store = profile_store;
        this.game_store = game_store;
        this.validator = validator;
        this.logger = logger;
    }

    public async Task<GameResponse> Handle(CollectionQuery query, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(query, cancellationToken);
        if (!validation_result.IsValid)
            return GameResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        ProfileDTO? profile = null;
        try
        { 
            profile = await profile_store.GetAsync(query.UserId, cancellationToken: cancellationToken);
        }
        catch (CosmosException e)
        {
            logger.LogError(e.Message);
        }
        if (profile == null)
            return GameResponse.Failure($"No profile found for {query.UserId}");

        var games = await game_store.GetAsync(profile.BoardGameIds, cancellationToken);
        return GameResponse.Success(games);
    }
}