using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components for a specific learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An object that can be cast to the specific result type.</returns>
    public static async Task<object> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var response = new GetLearningComponentsResponse(components);
            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            var errorResponse = new ErrorResponse(ex.Message);
            return TypedResults.BadRequest(errorResponse);
        }
        catch (KeyNotFoundException ex)
        {
            var errorResponse = new ErrorResponse(ex.Message);
            return TypedResults.NotFound(errorResponse);
        }
    }
}
