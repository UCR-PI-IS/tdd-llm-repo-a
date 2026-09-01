using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
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
    /// <param name="service">The whiteboard creation service.</param>
    /// <param name="request">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// A <see cref="Ok{T}"/> response with the created whiteboard,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// a <see cref="NotFound{T}"/> if the learning space does not exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> HandleAsync(
        [FromServices] IWhiteboardService service,
        [FromBody] CreateWhiteboardDto request)
    {
        try
        {
            var whiteboard = await service.CreateWhiteboardAsync(
                request.ComponentId,
                request.LearningSpaceId,
                request.Width,
                request.Height,
                request.Depth,
                request.X,
                request.Y,
                request.Z,
                request.Orientation,
                request.MarkerColor);

            var response = new CreateWhiteboardResponse(whiteboard);
            return TypedResults.Ok(response);
        }
        catch (Exception ex) when (ex is ArgumentException or ValidationException)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An unexpected error occurred while creating the whiteboard.");
        }
    }
}
