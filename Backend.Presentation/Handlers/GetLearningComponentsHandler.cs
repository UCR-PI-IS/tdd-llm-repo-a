using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Handler for fetching learning components by learning space ID.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the request to get learning components for a specific learning space.
    /// </summary>
    /// <param name="service">The learning component service</param>
    /// <param name="learningSpaceId">The ID of the learning space</param>
    /// <returns>An IResult containing the list of components or an error</returns>
    public static Task<IResult> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return Task.FromResult(ErrorResponse.BadRequestResult("Learning space ID cannot be null or empty"));
        }

        try
        {
            var components = service.GetComponentsByLearningSpaceIdAsync(learningSpaceId).Result;
            return Task.FromResult(GetLearningComponentsResponse.OkResult(components));
        }
        catch (AggregateException)
        {
            return Task.FromResult(ErrorResponse.NotFoundResult($"Learning space '{learningSpaceId}' not found"));
        }
    }
}
