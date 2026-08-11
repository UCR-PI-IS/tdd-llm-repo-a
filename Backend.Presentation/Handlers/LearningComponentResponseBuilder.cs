using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Builds success responses for learning component handlers.
/// </summary>
internal static class LearningComponentResponseBuilder
{
    /// <summary>
    /// Fetches components and builds the success response.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <returns>A learning components response.</returns>
    public static async Task<GetLearningComponentsResponse> BuildAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
        return new GetLearningComponentsResponse(
            LearningComponentMapper.ToDtoList(components));
    }
}
