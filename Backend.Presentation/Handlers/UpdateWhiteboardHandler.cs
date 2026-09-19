using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing whiteboard.
/// </summary>
public static class UpdateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to update an existing whiteboard.
    /// </summary>
    /// <param name="request">The update request containing whiteboard parameters.</param>
    /// <param name="service">The whiteboard update service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated whiteboard,
    /// or a <see cref="BadRequest{T}"/> if validation fails or the whiteboard is not found.
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
            return TypedResults.BadRequest(result.ErrorMessage);
        }

        var wb = result.Whiteboard!;
        var response = new UpdateWhiteboardResponse(
            wb.ComponentId,
            wb.LearningSpaceId,
            wb.Width,
            wb.Height,
            wb.Depth,
            wb.X,
            wb.Y,
            wb.Z,
            wb.Orientation,
            wb.MarkerColor);
        return TypedResults.Ok(response);
    }
}
