using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new whiteboard in a learning space.
/// </summary>
public static class CreateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new whiteboard.
    /// </summary>
    /// <param name="service">The whiteboard creation service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the created whiteboard,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// a <see cref="NotFound{T}"/> if the learning space does not exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> HandleAsync(
        IWhiteboardService service,
        CreateWhiteboardDto dto)
    {
        try
        {
            var request = new CreateWhiteboardRequest(
                dto.ComponentId,
                dto.LearningSpaceId,
                dto.Width,
                dto.Height,
                dto.Depth,
                dto.X,
                dto.Y,
                dto.Z,
                dto.Orientation,
                dto.MarkerColor);

            var whiteboard = await service.CreateWhiteboardAsync(request);
            return TypedResults.Ok(new CreateWhiteboardResponse(whiteboard));
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
