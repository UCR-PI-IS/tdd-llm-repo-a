using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
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
    /// <returns>A result object containing either the list of components or an error response.</returns>
    public static async Task<IResult> HandleAsync(ILearningComponentService service, string learningSpaceId)
    {
        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            var componentDtos = components.Select(c => new LearningComponentDto(
                c.ComponentId,
                c.LearningSpaceId,
                c.Width,
                c.Height,
                c.Depth,
                c.X,
                c.Y,
                c.Z,
                c.Orientation)).ToList();

            var response = new GetLearningComponentsResponse(componentDtos);
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
