using Microsoft.AspNetCore.Http.HttpResults;
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
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    public static async Task<object> HandleAsync(ILearningComponentService service, string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return Microsoft.AspNetCore.Http.Results.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
        }

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
            return Microsoft.AspNetCore.Http.Results.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return Microsoft.AspNetCore.Http.Results.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
