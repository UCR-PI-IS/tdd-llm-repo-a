using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using static Microsoft.AspNetCore.Http.Results;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components of a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a specific learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> containing the list of learning components or an error response.</returns>
    public static async Task<Microsoft.AspNetCore.Http.IResult> HandleAsync(ILearningComponentService service, string learningSpaceId)
    {
        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            return Ok(GetLearningComponentsResponse.FromEntities(components));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }
}
