using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Domain.Data;
using MediatR;

namespace BoardGameTracker.Application.Game.Commands;

public record class UpdateProfileCommand(Profile profile) : IRequest<Unit>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
{
    private IRepository<ProfileDTO> store;

    public UpdateProfileCommandHandler(IRepository<ProfileDTO> store)
    {
        this.store = store;
    }

    public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        //var dto = Mapping.Map(request.profile);
        //await store.UpdateAsync(dto, cancellationToken);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
