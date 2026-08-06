using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;

/// <summary>
/// Maps LearningSpace domain entities to presentation DTOs.
/// </summary>
public static class LearningSpaceMapper
{
    /// <summary>
    /// Maps a LearningSpace domain entity to a LearningSpaceDto.
    /// </summary>
    public static LearningSpaceDto ToDto(LearningSpace space)
    {
        return new LearningSpaceDto(space.id, space.type);
    }
}