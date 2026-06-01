using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching a list of learning components for a given learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch all learning components for a learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response containing the list of components,
    /// a <see cref="BadRequest{T}"/> if the learning space ID is invalid,
    /// or a <see cref="NotFound{T}"/> if the learning space does not exist.
    /// </returns>
    public static async Task<Microsoft.AspNetCore.Http.IResult> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return HandlerResponses.BadRequest("Learning space ID cannot be null or empty.");
        }

        var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        if (components.Count == 0 && !HandlerResponses.IsKnownLearningSpace(learningSpaceId))
        {
            return HandlerResponses.NotFound($"No learning components found for learning space '{learningSpaceId}'.");
        }

        var response = LearningComponentMapper.ToResponse(components);

        return HandlerResponses.Ok(response);
    }
}