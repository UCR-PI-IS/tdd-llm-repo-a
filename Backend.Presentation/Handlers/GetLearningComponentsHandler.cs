using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components of a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A response containing the list of learning components or an error.</returns>
    public static async Task<object> HandleAsync(ILearningComponentService learningComponentService, String learningSpaceId)
    {
        try
        {
            return TypedResults.Ok(
                GetLearningComponentsResponse.FromEntities(
                    await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId)));
        }
        catch (Exception ex)
        {
            return ErrorResponse.FromException(ex);
        }
    }
}
