using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Helpers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching a list of learning components for a specific learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a specific learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An appropriate HTTP response with the list of components or an error.</returns>
    public static Task<IResult> HandleAsync(
        ILearningComponentListService service,
        string learningSpaceId)
    {
        return RequestHandler.ExecuteWithErrorResponseAsync(
            () => service.GetComponentsByLearningSpaceIdAsync(learningSpaceId),
            components => LearningComponentMapper.ToResponse(components));
    }
}
