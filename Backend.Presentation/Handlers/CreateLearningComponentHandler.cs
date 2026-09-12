using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning component.
/// </summary>
public static class CreateLearningComponentHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new learning component.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    /// <param name="request">The creation request.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the created component details,
    /// a <see cref="Conflict{T}"/> if the ID already exists,
    /// or a <see cref="BadRequest{T}"/> if validation fails.
    /// </returns>
    public static Task<IResult> HandleAsync(
        ILearningComponentService service,
        CreateComponentRequest request)
    {
        return CreateLearningComponentResponseFactory.CreateResponseAsync(service, request);
    }
}
