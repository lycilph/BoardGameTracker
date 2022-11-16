using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Game.DTO;
using MediatR;

namespace BoardGameTracker.Application.Game.Queries;

public record class ProfileQuery(string Id) : IRequest<ProfileDTO>;

public class ProfileQueryHandler : IRequestHandler<ProfileQuery, ProfileDTO>
{
    private readonly IRepository<ProfileDTO> store;

    public ProfileQueryHandler(IRepository<ProfileDTO> store)
    {
        this.store = store;
    }

    public async Task<ProfileDTO> Handle(ProfileQuery request, CancellationToken cancellationToken)
    {
        if (await store.ExistsAsync(request.Id, cancellationToken: cancellationToken))
            return await store.GetAsync(request.Id, cancellationToken: cancellationToken);

        return new ProfileDTO { Id = request.Id };
    }
}