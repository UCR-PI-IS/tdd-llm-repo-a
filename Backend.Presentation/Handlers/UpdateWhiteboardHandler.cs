using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing whiteboard in a learning space.
/// </summary>
public static class UpdateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to update an existing whiteboard.
    /// </summary>
    /// <param name="id">The identifier of the whiteboard to update.</param>
    /// <param name="request">The HTTP request.</param>
    /// <param name="service">The whiteboard update service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated whiteboard,
    /// or a <see cref="BadRequest{T}"/> if validation fails or the whiteboard is not found.
    /// </returns>
    public static async Task<Results<Ok<UpdateWhiteboardResponse>, BadRequest<string>>> HandleAsync(
        string id,
        HttpRequest request,
        IWhiteboardUpdateService service)
    {
        UpdateWhiteboardDto dto;
        try
        {
            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                dto = new UpdateWhiteboardDto();
            }
            else
            {
                dto = JsonSerializer.Deserialize<UpdateWhiteboardDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new UpdateWhiteboardDto();
            }
        }
        catch
        {
            dto = new UpdateWhiteboardDto();
        }

        var updateRequest = new UpdateWhiteboardRequest(
            id,
            dto.Width,
            dto.Height,
            dto.Depth,
            dto.X,
            dto.Y,
            dto.Z,
            dto.Orientation,
            dto.MarkerColor);

        var result = await service.UpdateWhiteboardAsync(updateRequest);

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(result.ErrorMessage!);
        }

        var wb = result.Value!;
        var whiteboardDto = new WhiteboardDto(
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

        return TypedResults.Ok(new UpdateWhiteboardResponse(whiteboardDto));
    }
}
