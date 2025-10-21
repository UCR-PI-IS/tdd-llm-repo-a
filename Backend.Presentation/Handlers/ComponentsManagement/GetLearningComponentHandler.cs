using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.ComponentsManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.ComponentsManagement;

/// <summary>
/// Handler for retrieving all learning components (whiteboards, projectors, etc.).
/// </summary>
public static class GetLearningComponentHandler
{
    /// <summary>
    /// Handles the request to retrieve all learning components in the system.
    /// </summary>
    /// <param name="learningComponentService">Service to access learning component data.</param>
    /// <returns>
    /// A list of <see cref="GetLearningComponentResponse"/> wrapped in a 200 OK result,
    /// or 404 NotFound if no components are available.
    /// </returns>
    public static async Task<Results<Ok<GetLearningComponentResponse>, Conflict, BadRequest<ErrorResponse>>> HandleAsync(
        [FromServices] ILearningComponentServices learningComponentService)
    {
        var components = await learningComponentService.GetLearningComponentAsync();
        var dtoList = new List<LearningComponentDto>();

        List<string> errorMessages = new();

        try
        {
            dtoList = components
                 .Select<LearningComponent, LearningComponentDto>(component =>
                 {
                     return component switch
                     {
                         Projector projector => ProjectorDtoMapper.ToDto(projector),
                         Whiteboard whiteboard => WhiteboardDtoMapper.ToDto(whiteboard),
                         _ => throw new InvalidCastException("Unknown component type.")
                     };
                 })
                .ToList();
        }
        catch (ValidationException exception)
        {
            errorMessages.Add(exception.Message);
        }

        if (errorMessages.Count > 0)
        {
            return TypedResults.BadRequest(
                new ErrorResponse(errorMessages));
        }

        var response = new GetLearningComponentResponse(dtoList);
        return TypedResults.Ok(response);
    }
}


    