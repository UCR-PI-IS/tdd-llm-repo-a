using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Requests;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing whiteboard.
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
    /// or a <see cref="BadRequest{T}"/> if validation fails or whiteboard not found.
    /// </returns>
    public static async Task<Results<Ok<UpdateWhiteboardResponse>, BadRequest<string>>> HandleAsync(
        UpdateWhiteboardRequest request,
        IWhiteboardService service)
    {
        var dto = new UpdateWhiteboardDto(
            request.WhiteboardId,
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
            return TypedResults.BadRequest(result.ErrorMessage!);
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
