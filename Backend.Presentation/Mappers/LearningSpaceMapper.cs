using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps domain entities to presentation DTOs and responses for learning spaces.
/// </summary>
internal static class LearningSpaceMapper
{
    /// <summary>
    /// Maps a collection of LearningSpace entities to a GetLearningSpaceListResponse.
    /// </summary>
    /// <param name="spaces">The domain entities to map.</param>
    /// <returns>A response containing the mapped DTOs.</returns>
    internal static GetLearningSpaceListResponse ToResponse(IEnumerable<LearningSpace> spaces)
    {
        var dtos = spaces.Select(ToDto).ToList();
        return new GetLearningSpaceListResponse(dtos);
    }

    /// <summary>
    /// Maps a LearningSpace entity to a LearningSpaceDto.
    /// </summary>
    /// <param name="space">The domain entity to map.</param>
    /// <returns>The mapped DTO.</returns>
    internal static LearningSpaceDto ToDto(LearningSpace space)
    {
        return new LearningSpaceDto(space.id, space.type);
    }
}