using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
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
    /// <param name="learningSpaceId">The unique identifier of the learning space.</param>
    /// <returns>An <see cref="Ok{T}"/> response containing the list of learning components, or an error response.</returns>
    public static async Task<IResult> HandleAsync(
        [FromServices] ILearningComponentService service,
        [FromRoute] string learningSpaceId)
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
        catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
        {
            return TypedResults.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
