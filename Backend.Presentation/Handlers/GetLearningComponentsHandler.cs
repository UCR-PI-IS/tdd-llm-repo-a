using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components of a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Maps a LearningComponent entity to a LearningComponentDto.
    /// </summary>
    /// <param name="component">The learning component entity.</param>
    /// <returns>A LearningComponentDto with mapped values.</returns>
    private static LearningComponentDto MapToDto(LearningComponent component)
    {
        return new LearningComponentDto(
            component.ComponentId,
            component.LearningSpaceId,
            component.Width,
            component.Height,
            component.Depth,
            component.X,
            component.Y,
            component.Z,
            component.Orientation);
    }

    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An <see cref="IResult"/> response containing the list of learning components or an error.</returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            return TypedResults.BadRequest(
                new ErrorResponse("Learning space ID cannot be null or empty."));
        }

        try
        {
            // Fetch components from service
            var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            // Map to DTOs and create response
            var response = new GetLearningComponentsResponse(
                components.Select(MapToDto).ToList()
            );

            return TypedResults.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
