using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps LearningSpace entities to DTOs.
/// </summary>
public static class LearningSpaceMapper
{
    /// <summary>
    /// Maps a LearningSpace entity to a LearningSpaceDto.
    /// </summary>
    /// <param name="space">The learning space entity.</param>
    /// <returns>A LearningSpaceDto.</returns>
    public static LearningSpaceDto ToDto(LearningSpace space)
    {
        return new LearningSpaceDto(space.id, space.type);
    }

    /// <summary>
    /// Maps a collection of LearningSpace entities to a list of LearningSpaceDto objects.
    /// </summary>
    /// <param name="spaces">The collection of learning space entities.</param>
    /// <returns>A list of LearningSpaceDto objects.</returns>
    public static List<LearningSpaceDto> ToDtoList(IEnumerable<LearningSpace> spaces)
    {
        return spaces.Select(ToDto).ToList();
    }
}
