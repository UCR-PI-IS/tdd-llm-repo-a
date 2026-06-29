using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Builds responses for learning component requests.
/// </summary>
public static class LearningComponentResponseBuilder
{
    /// <summary>
    /// Creates a BadRequest response with an error message.
    /// </summary>
    public static IResult BadRequest(string message) =>
        TypedResults.BadRequest(new ErrorResponse(message));

    /// <summary>
    /// Creates a NotFound response with an error message.
    /// </summary>
    public static IResult NotFound(string message) =>
        TypedResults.NotFound(new ErrorResponse(message));

    /// <summary>
    /// Handles the core async logic for fetching components and building the OK response.
    /// </summary>
    public static async Task<IResult> HandleCoreAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        try
        {
            var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var componentDtos = components.Select(MapToDto).ToList();
            var response = new GetLearningComponentsResponse(componentDtos);
            return TypedResults.Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Learning space '{learningSpaceId}' not found");
        }
    }

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
}
