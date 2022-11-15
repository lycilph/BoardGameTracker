using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Domain.Data;
using MediatR;

namespace BoardGameTracker.Application.Game.Queries;

public record class ProfileQuery(string Id) : IRequest<Profile>;

public class ProfileQueryHandler : IRequestHandler<ProfileQuery, Profile>
{
    private IRepository<ProfileDTO> store;

    public ProfileQueryHandler(IRepository<ProfileDTO> store)
    {
        this.store = store;
    }

    public async Task<Profile> Handle(ProfileQuery request, CancellationToken cancellationToken)
    {
        //var pk = new PartitionKey(request.Id);
        //if (await store.ExistsAsync(request.Id, pk, cancellationToken)) 
        //{ 
        //    var dto = await store.GetAsync(request.Id, pk, cancellationToken);
        //    return Mapping.Map(dto);
        //}
        //else
        //{
        //    var profile = new Profile { Id = request.Id };
        //    var dto = Mapping.Map(profile);
        //    await store.UpdateAsync(dto, cancellationToken);
        //    return profile;
        //}

        await Task.CompletedTask;
        return new Profile();
    }
}

