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
    /// <param name="service">The learning space create service.</param>
    /// <param name="dto">The data transfer object containing the creation data.</param>
    /// <returns>A result indicating success or failure of the creation.</returns>
    public static Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> HandleAsync(
        [FromServices] ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        return CreateLearningSpaceExecutor.ExecuteAsync(service, dto);
    }
}
