using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning spaces.
/// </summary>
public record class GetLearningSpaceListResponse(List<LearningSpaceDto> LearningSpaces)
{
    /// <summary>
    /// Creates a response from a collection of learning space entities.
    /// </summary>
    /// <param name="spaces">The learning space entities.</param>
    /// <returns>A GetLearningSpaceListResponse.</returns>
    public static GetLearningSpaceListResponse FromEntities(IEnumerable<LearningSpace> spaces)
    {
        return new GetLearningSpaceListResponse(
            spaces.Select(s => new LearningSpaceDto(s.id, s.type)).ToList());
    }
}
