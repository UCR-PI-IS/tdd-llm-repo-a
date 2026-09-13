using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
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
    /// <returns>A <see cref="CreateLearningComponentResponse"/> with the result of the operation.</returns>
    public static async Task<CreateLearningComponentResponse> HandleAsync(
        ILearningComponentService service,
        CreateLearningComponentDto dto)
    {
        try
        {
            var request = LearningComponentMapper.ToCreateRequest(dto);
            var component = await service.CreateComponentAsync(request);
            var message = string.IsNullOrEmpty(dto.componentId)
                ? $"Component created successfully with auto-generated ID {component.ComponentId}"
                : $"Component created successfully with ID {component.ComponentId}";

            return new CreateLearningComponentResponse(
                ComponentId: component.ComponentId,
                Message: message,
                StatusCode: 201);
        }
        catch (DuplicateIdException ex)
        {
            return new CreateLearningComponentResponse(
                StatusCode: 409,
                ErrorMessage: ex.Message);
        }
        catch (ValidationException ex)
        {
            return new CreateLearningComponentResponse(
                StatusCode: 400,
                ErrorMessage: ex.Message);
        }
    }
}
