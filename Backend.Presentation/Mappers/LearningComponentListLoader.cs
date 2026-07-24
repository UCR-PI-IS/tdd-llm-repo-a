using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Loads learning components and builds the list response DTO.
/// </summary>
internal static class LearningComponentListLoader
{
    /// <summary>
    /// Loads components for the learning space and maps them to a response.
    /// </summary>
    /// <param name="learningComponentService">Service used to load components.</param>
    /// <param name="learningSpaceId">Learning space identifier.</param>
    public static GetLearningComponentsResponse Load(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        var components = learningComponentService
            .GetComponentsByLearningSpaceIdAsync(learningSpaceId)
            .GetAwaiter()
            .GetResult();

        return new GetLearningComponentsResponse(LearningComponentDtoMapper.ToDtoList(components));
    }
}
