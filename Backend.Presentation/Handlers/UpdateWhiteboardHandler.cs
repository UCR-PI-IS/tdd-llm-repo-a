using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Request object carrying the parameters needed to update a whiteboard.
/// </summary>
/// <param name="ComponentId">Unique identifier for the whiteboard.</param>
/// <param name="Width">Width of the whiteboard in meters.</param>
/// <param name="Height">Height of the whiteboard in meters.</param>
/// <param name="Depth">Depth of the whiteboard in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the whiteboard.</param>
/// <param name="MarkerColor">Color of the whiteboard marker.</param>
public record class UpdateWhiteboardRequest(
    string ComponentId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);

/// <summary>
/// Response object carrying the updated whiteboard data.
/// </summary>
/// <param name="ComponentId">Unique identifier for the whiteboard.</param>
/// <param name="LearningSpaceId">Identifier of the learning space.</param>
/// <param name="Width">Width of the whiteboard in meters.</param>
/// <param name="Height">Height of the whiteboard in meters.</param>
/// <param name="Depth">Depth of the whiteboard in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the whiteboard.</param>
/// <param name="MarkerColor">Color of the whiteboard marker.</param>
public record class UpdateWhiteboardResponse(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);

/// <summary>
/// Handler for updating an existing whiteboard in a learning space.
/// </summary>
public static class UpdateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to update a whiteboard.
    /// </summary>
    /// <param name="request">The update request containing whiteboard parameters.</param>
    /// <param name="service">The whiteboard service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated whiteboard,
    /// or a <see cref="BadRequest{T}"/> if validation fails or the whiteboard is not found.
    /// </returns>
    public static async Task<Results<Ok<UpdateWhiteboardResponse>, BadRequest<string>>> HandleAsync(
        UpdateWhiteboardRequest request,
        IWhiteboardService service)
    {
        var dto = new UpdateWhiteboardDto(
            request.ComponentId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation,
            request.MarkerColor);

        var result = await service.UpdateWhiteboardAsync(dto);

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(result.ErrorMessage);
        }

        var whiteboard = result.Whiteboard!;
        var response = new UpdateWhiteboardResponse(
            whiteboard.ComponentId,
            whiteboard.LearningSpaceId,
            whiteboard.Width,
            whiteboard.Height,
            whiteboard.Depth,
            whiteboard.X,
            whiteboard.Y,
            whiteboard.Z,
            whiteboard.Orientation,
            whiteboard.MarkerColor);

        return TypedResults.Ok(response);
    }
}
