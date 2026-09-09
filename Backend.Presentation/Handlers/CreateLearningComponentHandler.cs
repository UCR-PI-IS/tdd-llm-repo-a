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
    /// <param name="dto">The DTO carrying the creation data.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the created component,
    /// a <see cref="Conflict{T}"/> if the ID already exists,
    /// or a <see cref="BadRequest{T}"/> if validation fails.
    /// </returns>
    public static async Task<Results<Ok<CreateLearningComponentResponse>, Conflict<ErrorResponse>, BadRequest<ErrorResponse>>> HandleAsync(
        ILearningComponentService service,
        CreateLearningComponentDto dto)
    {
        try
        {
            var request = new CreateComponentRequest(
                ComponentId: dto.ComponentId,
                LearningSpaceId: dto.LearningSpaceId,
                Width: dto.Width,
                Height: dto.Height,
                Depth: dto.Depth,
                X: dto.X,
                Y: dto.Y,
                Z: dto.Z,
                Orientation: dto.Orientation);

            var component = await service.CreateComponentAsync(request);

            var response = new CreateLearningComponentResponse(
                ComponentId: component.ComponentId,
                LearningSpaceId: component.LearningSpaceId,
                Width: component.Width,
                Height: component.Height,
                Depth: component.Depth,
                X: component.X,
                Y: component.Y,
                Z: component.Z,
                Orientation: component.Orientation);

            return Microsoft.AspNetCore.Http.TypedResults.Ok(response);
        }
        catch (DuplicateIdException ex)
        {
            return Microsoft.AspNetCore.Http.TypedResults.Conflict(new ErrorResponse(ex.Message));
        }
        catch (ValidationException ex)
        {
            return Microsoft.AspNetCore.Http.TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }
    }
}
