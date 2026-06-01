using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching a list of learning components for a given learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning component data.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// An OK response containing the list of components,
    /// a BadRequest response if the learning space ID is invalid,
    /// or a NotFound response if the learning space does not exist.
    /// </returns>
    public static async Task<object> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return LearningComponentResponseBuilder.ToBadRequestResponse("Learning space ID cannot be null or empty.");
        }

        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            return LearningComponentResponseBuilder.ToOkResponse(components);
        }
        catch (KeyNotFoundException)
        {
            return LearningComponentResponseBuilder.ToNotFoundResponse($"Learning space '{learningSpaceId}' not found.");
        }
    }
}