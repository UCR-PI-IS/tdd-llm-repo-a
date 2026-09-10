using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Interface for handling create learning component requests.
/// </summary>
public interface ICreateLearningComponentHandler
{
    /// <summary>
    /// Handles the create component request asynchronously.
    /// </summary>
    /// <param name="request">The create component request.</param>
    /// <returns>An HTTP result.</returns>
    Task<IResult> HandleAsync(CreateComponentRequest request);
}
