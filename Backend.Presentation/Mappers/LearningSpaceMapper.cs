using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class LearningSpaceMapper
{
    public static List<LearningSpaceDto> ToDtoList(IEnumerable<LearningSpace> spaces)
    {
        return spaces.Select(space => new LearningSpaceDto(
            space.LearningSpaceId.ToString(),
            space.Type)).ToList();
    }
}
