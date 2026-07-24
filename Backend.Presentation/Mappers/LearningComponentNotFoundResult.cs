using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Builds NotFound results for learning component endpoints.
/// </summary>
internal static class LearningComponentNotFoundResult
{
    /// <summary>
    /// Creates a NotFound result with the given message.
    /// </summary>
    /// <param name="message">Error message to return.</param>
    public static IResult Create(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }
}
