using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning space.
/// </summary>
public static class CreateLearningSpaceHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new learning space.
    /// </summary>
    /// <param name="service">The learning space creation service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// A <see cref="Created{T}"/> response with the created learning space,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> HandleAsync(
        [FromServices] ILearningSpaceCreateService service,
        [FromBody] CreateLearningSpaceDto dto)
    {
        return LearningSpaceResponseFactory.ExecuteAsync(
            service, dto.Type, dto.Height, dto.Width, dto.Length);
    }
}
