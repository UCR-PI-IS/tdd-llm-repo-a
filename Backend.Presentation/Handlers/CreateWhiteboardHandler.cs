using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Wrapper for handler results that carry an <see cref="IResult"/>.
/// </summary>
/// <param name="Result">The HTTP result.</param>
public record CreateWhiteboardHandlerResult(IResult Result);

/// <summary>
/// Handler for creating a whiteboard in a learning space.
/// </summary>
public static class CreateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a whiteboard.
    /// </summary>
    /// <param name="whiteboardCreateService">Service for creating whiteboards.</param>
    /// <param name="dto">The creation request DTO.</param>
    /// <returns>
    /// A <see cref="CreateWhiteboardHandlerResult"/> wrapping an <see cref="Ok{T}"/> response containing the created whiteboard,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// a <see cref="NotFound{T}"/> if the learning space does not exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static async Task<CreateWhiteboardHandlerResult> HandleAsync(
        IWhiteboardCreateService whiteboardCreateService,
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

            var whiteboard = await whiteboardCreateService.CreateWhiteboardAsync(request);

            var response = new CreateWhiteboardResponse(
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

            return new CreateWhiteboardHandlerResult(TypedResults.Ok(response));
        }
        catch (ArgumentException ex)
        {
            return new CreateWhiteboardHandlerResult(TypedResults.BadRequest(ex.Message));
        }
        catch (NotFoundException ex)
        {
            return new CreateWhiteboardHandlerResult(TypedResults.NotFound(ex.Message));
        }
        catch (ValidationException ex)
        {
            return new CreateWhiteboardHandlerResult(TypedResults.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return new CreateWhiteboardHandlerResult(TypedResults.Problem(
                detail: "An unexpected error occurred.",
                statusCode: 500));
        }
    }
}
