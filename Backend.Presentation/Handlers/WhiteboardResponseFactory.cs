using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses related to whiteboard creation operations.
/// </summary>
internal static class WhiteboardResponseFactory
{
    /// <summary>
    /// Executes the whiteboard creation workflow and returns the appropriate HTTP result.
    /// </summary>
    /// <param name="service">The whiteboard service.</param>
    /// <param name="dto">The data transfer object containing creation parameters.</param>
    /// <returns>The appropriate HTTP result for the operation outcome.</returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> ExecuteAsync(
        IWhiteboardService service,
        CreateWhiteboardDto dto)
    {
        try
        {
            var request = new CreateWhiteboardRequest(
                dto.ComponentId, dto.LearningSpaceId,
                dto.Width, dto.Height, dto.Depth,
                dto.X, dto.Y, dto.Z,
                dto.Orientation, dto.MarkerColor);

            var whiteboard = await service.CreateWhiteboardAsync(request);
            return TypedResults.Ok(new CreateWhiteboardResponse(whiteboard));
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    private static Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult> MapException(Exception ex)
    {
        return ex switch
        {
            ArgumentException => TypedResults.BadRequest(ex.Message),
            NotFoundException => TypedResults.NotFound(ex.Message),
            ValidationException => TypedResults.BadRequest(ex.Message),
            _ => TypedResults.Problem("An unexpected error occurred while creating the whiteboard.")
        };
    }
}
