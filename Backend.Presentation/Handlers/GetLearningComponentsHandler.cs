using static Microsoft.AspNetCore.Http.TypedResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components by learning space ID.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a specific learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An result response containing the list of learning components or an error.</returns>
    public static async Task<object> HandleAsync(ILearningComponentService service, string learningSpaceId)
    {
        // Validate input
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
        }

        try
        {
            // Fetch components from the service
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            // Create and return the response
            var response = new GetLearningComponentsResponse(components);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}
