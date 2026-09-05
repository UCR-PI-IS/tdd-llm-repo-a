using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new whiteboard.
/// </summary>
public static class CreateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new whiteboard.
    /// </summary>
    /// <param name="service">The whiteboard service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// A <see cref="Ok{T}"/> response with the created whiteboard,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// a <see cref="NotFound{T}"/> if the learning space doesn't exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> HandleAsync(
        [FromServices] IWhiteboardService service,
        [FromBody] CreateWhiteboardDto dto)
    {
        try
        {
            var request = new CreateWhiteboardRequest(
                dto.LearningSpaceId,
                dto.ComponentId,
                dto.Width,
                dto.Height,
                dto.Depth,
                dto.X,
                dto.Y,
                dto.Z,
                dto.Orientation,
                dto.MarkerColor);

            var whiteboard = await service.CreateWhiteboardAsync(request);

            var responseDto = new CreateWhiteboardDto(
                whiteboard.LearningSpaceId,
                whiteboard.ComponentId,
                whiteboard.Width,
                whiteboard.Height,
                whiteboard.Depth,
                whiteboard.X,
                whiteboard.Y,
                whiteboard.Z,
                whiteboard.Orientation,
                whiteboard.MarkerColor);

            var response = new CreateWhiteboardResponse(responseDto);
            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An unexpected error occurred while creating the whiteboard.");
        }
    }
}
