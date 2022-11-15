using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.Game.Services;

public static class Mapping
{
    public static ProfileDTO Map(Profile profile)
    {
        return new ProfileDTO
        {
            Id = profile.Id,
            LastUsedBGGUsername= profile.LastUsedBGGUsername,
        };
    }

    public static Profile Map(ProfileDTO dto)
    {
        return new Profile
        {
            Id = dto.Id,
            LastUsedBGGUsername = dto.LastUsedBGGUsername,
        };
    }
}
