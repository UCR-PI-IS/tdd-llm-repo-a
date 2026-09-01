using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Static handler class for creating whiteboards.
/// </summary>
public static class CreateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new whiteboard.
    /// </summary>
    /// <param name="service">The whiteboard service.</param>
    /// <param name="request">The request containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="IResult"/> representing the HTTP response.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        IWhiteboardService service,
        CreateWhiteboardRequest request)
    {
        try
        {
            var whiteboard = await service.CreateWhiteboardAsync(request);
            return Results.Created($"/api/whiteboards/{whiteboard.ComponentId}", new CreateWhiteboardResponse(whiteboard));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return Results.Problem("An unexpected error occurred.", statusCode: 500);
        }
    }
}
