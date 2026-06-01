using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Builds HTTP responses for learning component operations.
/// </summary>
public static class LearningComponentResponseBuilder
{
    /// <summary>
    /// Builds an OK response containing the list of learning components.
    /// </summary>
    public static Ok<GetLearningComponentsResponse> ToOkResponse(List<LearningComponent> components)
    {
        return TypedResults.Ok(LearningComponentMapper.ToResponse(components));
    }

    /// <summary>
    /// Builds a BadRequest response with the given error message.
    /// </summary>
    public static BadRequest<ErrorResponse> ToBadRequestResponse(string message)
    {
        return TypedResults.BadRequest(new ErrorResponse(message));
    }

    /// <summary>
    /// Builds a NotFound response with the given error message.
    /// </summary>
    public static NotFound<ErrorResponse> ToNotFoundResponse(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }
}