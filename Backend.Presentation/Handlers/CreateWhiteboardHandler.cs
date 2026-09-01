using Microsoft.AspNetCore.Http.HttpResults;
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
    /// <param name="service">The whiteboard service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the created whiteboard,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// a <see cref="NotFound{T}"/> if the learning space does not exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> HandleAsync(
        IWhiteboardService service,
        CreateWhiteboardDto dto)
    {
        return WhiteboardResponseFactory.ExecuteAsync(service, dto);
    }
}
