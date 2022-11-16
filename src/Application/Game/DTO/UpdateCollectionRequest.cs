using FluentValidation;

namespace BoardGameTracker.Application.Game.DTO;

public class UpdateCollectionRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<BoardGameDTO> Games { get; set; } = new();
}

public class UpdateCollectionRequestValidator : AbstractValidator<UpdateCollectionRequest>
{
    public UpdateCollectionRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Games).NotEmpty().WithMessage("No games to update");
    }
}