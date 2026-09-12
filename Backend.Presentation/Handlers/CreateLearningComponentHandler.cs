using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning component in a learning space.
/// </summary>
public static class CreateLearningComponentHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new learning component.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    /// <param name="request">The request containing the creation parameters.</param>
    /// <returns>
    /// A <see cref="Created{T}"/> response with the created component details,
    /// a <see cref="Conflict{T}"/> if the ID already exists,
    /// or a <see cref="BadRequest{T}"/> if validation fails.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService service,
        CreateComponentRequest request)
    {
        try
        {
            var result = await service.CreateComponentAsync(request);
            return CreateComponentResponseFactory.Created(result);
        }
        catch (ComponentCreationException ex)
        {
            return CreateComponentResponseFactory.FromException(ex);
        }
    }
}
