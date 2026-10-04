using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

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
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the created component,
    /// a <see cref="Conflict{T}"/> if the ID already exists,
    /// or a <see cref="BadRequest{T}"/> if validation fails.
    /// </returns>
    public static async Task<Results<Ok<CreateLearningComponentResponse>, Conflict<string>, BadRequest<string>>> HandleAsync(
        ILearningComponentService service,
        CreateLearningComponentDto dto)
    {
        try
        {
            var request = new CreateComponentRequest(
                dto.ComponentId,
                dto.LearningSpaceId,
                dto.Width, dto.Height, dto.Depth,
                dto.X, dto.Y, dto.Z,
                dto.Orientation);

            var component = await service.CreateComponentAsync(request);
            var response = new CreateLearningComponentResponse(component.ComponentId);
            return TypedResults.Ok(response);
        }
        catch (DuplicateIdException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
