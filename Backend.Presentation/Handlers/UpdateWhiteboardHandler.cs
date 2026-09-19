using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing whiteboard in a learning space.
/// </summary>
public static class UpdateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to update a whiteboard.
    /// </summary>
    /// <param name="service">The whiteboard service.</param>
    /// <param name="request">The update request containing the new parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated whiteboard,
    /// or a <see cref="BadRequest{T}"/> if validation fails or the whiteboard is not found.
    /// </returns>
    public static async Task<Results<Ok<UpdateWhiteboardResponse>, BadRequest<string>>> HandleAsync(
        IWhiteboardService service,
        UpdateWhiteboardRequest request)
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

        var response = WhiteboardMapper.ToUpdateResponse(result.Whiteboard!);
        return TypedResults.Ok(response);
    }
}
