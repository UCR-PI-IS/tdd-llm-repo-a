using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using System.ComponentModel;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.Spaces;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.ComponentsManagement;

/// <summary>
/// Handler for retrieving learning components for a specific learning space.
/// </summary>
public static class GetLearningComponentsByIdHandler
{
    /// <summary>
    /// Retrieves learning components based on building, floor, and learning space identifiers.
    /// </summary>
    /// <param name="learningComponentService">Injected service for retrieving components.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <returns>Filtered list of components in the specified learning space, or 404 if none found.</returns>
    public static async Task<Results<Ok<GetLearningComponentsByIdResponse>, Conflict, BadRequest<List<ValidationError>>>> HandleAsync(
        [FromServices] ILearningComponentServices learningComponentService,
        [FromRoute(Name = "learningSpaceId")] int routeLearningSpaceId)
    {
        var errorMessages = new List<ValidationError>();

        // Validate LearningSpaceID
        if (!Id.TryCreate(routeLearningSpaceId, out var learningSpaceId))
            errorMessages.Add(new ValidationError("Learning Space Id","Invalid learning space id format."));


        var dtoList = new List<LearningComponentDto>();

        var components = await learningComponentService.GetLearningComponentsByIdAsync(routeLearningSpaceId);

        try
        {
            dtoList = components
                .Select<LearningComponent, LearningComponentDto>(component => {
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
            errorMessages.AddRange(exception.Errors);
        }

        if (errorMessages.Count > 0)
        {
            return TypedResults.BadRequest(errorMessages);
        }

        return TypedResults.Ok(new GetLearningComponentsByIdResponse(dtoList));
    }
}