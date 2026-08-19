using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses related to learning component operations.
/// </summary>
internal static class LearningComponentResponseFactory
{
    /// <summary>
    /// Creates an OK response containing the list of learning component DTOs.
    /// </summary>
    /// <param name="components">The domain entities to include in the response.</param>
    /// <returns>An <see cref="Ok{T}"/> result containing the response.</returns>
    public static IResult CreateOkResponse(List<LearningComponent> components)
    {
        var dtos = LearningComponentMapper.ToDtoList(components);
        var response = new GetLearningComponentsResponse(dtos);
        return TypedResults.Ok(response);
    }

    /// <summary>
    /// Creates a BadRequest response for an invalid learning space ID.
    /// </summary>
    /// <returns>A <see cref="BadRequest{T}"/> result with an error message.</returns>
    public static IResult CreateBadRequestResponse()
    {
        return TypedResults.BadRequest(
            new ErrorResponse("Learning space ID cannot be null or empty"));
    }

    /// <summary>
    /// Creates a NotFound response for a learning space that does not exist.
    /// </summary>
    /// <param name="message">The error message to include in the response.</param>
    /// <returns>A <see cref="NotFound{T}"/> result with an error message.</returns>
    public static IResult CreateNotFoundResponse(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }
}
