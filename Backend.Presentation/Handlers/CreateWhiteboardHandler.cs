using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
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
    /// a <see cref="NotFound{T}"/> if the learning space doesn't exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> HandleAsync(
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
        catch (Exception ex)
        {
            return WhiteboardErrorHandler.Handle(ex);
        }
    }
}
