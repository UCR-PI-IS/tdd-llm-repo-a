using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
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
    /// <param name="service">The whiteboard creation service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>The appropriate HTTP result for the operation outcome.</returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> ExecuteAsync(
        IWhiteboardCreateService service,
        CreateWhiteboardDto dto)
    {
        try
        {
            var request = WhiteboardMapper.ToRequest(dto);
            var whiteboard = await service.CreateWhiteboardAsync(request);
            var response = WhiteboardMapper.ToResponse(whiteboard);
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
            return TypedResults.Problem(
                detail: "An unexpected error occurred.",
                statusCode: 500);
        }
    }
}
