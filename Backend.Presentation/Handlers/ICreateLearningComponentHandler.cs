using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Interface for the create learning component handler.
/// </summary>
public interface ICreateLearningComponentHandler
{
    /// <summary>
    /// Handles the request to create a new learning component.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
    Task<IResult> HandleAsync(CreateComponentRequest request);
}
