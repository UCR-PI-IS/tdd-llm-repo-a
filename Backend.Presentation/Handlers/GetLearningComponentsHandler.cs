using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components of a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a specific learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning component data.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An appropriate response based on the result of the operation.</returns>
    public static async Task<object> HandleAsync(
        [FromServices] ILearningComponentService service, 
        string learningSpaceId)
    {
        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            var response = new GetLearningComponentsResponse(
                components.Select(c => new LearningComponentDto(
                    c.ComponentId,
                    c.LearningSpaceId,
                    c.Width,
                    c.Height,
                    c.Depth,
                    c.X,
                    c.Y,
                    c.Z,
                    c.Orientation)).ToList()
            );

            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
