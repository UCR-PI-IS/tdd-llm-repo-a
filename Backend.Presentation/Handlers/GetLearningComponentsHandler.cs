using Microsoft.AspNetCore.Http;
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
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <returns>An <see cref="Ok{T}"/> response containing the list of learning components, or an error response.</returns>
    public static async Task<object> HandleAsync(ILearningComponentService service, string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return Microsoft.AspNetCore.Http.Results.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty."));
        }

        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var response = new GetLearningComponentsResponse(components);
            return Microsoft.AspNetCore.Http.Results.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return Microsoft.AspNetCore.Http.Results.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
