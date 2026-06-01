using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Provides factory methods for creating common HTTP result responses and validation helpers.
/// </summary>
internal static class HandlerResponses
{
    /// <summary>
    /// Creates a BadRequest result with the specified message.
    /// </summary>
    public static IResult BadRequest(string message)
    {
        return TypedResults.BadRequest(new ErrorResponse(message));
    }

    /// <summary>
    /// Creates a NotFound result with the specified message.
    /// </summary>
    public static IResult NotFound(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }

    /// <summary>
    /// Creates an Ok result with the specified value.
    /// </summary>
    public static IResult Ok<T>(T value)
    {
        return TypedResults.Ok(value);
    }

    /// <summary>
    /// Checks whether the given learning space ID corresponds to a known learning space.
    /// </summary>
    /// <param name="learningSpaceId">The learning space ID to check.</param>
    /// <returns>True if the learning space is known to exist; otherwise, false.</returns>
    public static bool IsKnownLearningSpace(string learningSpaceId)
    {
        return learningSpaceId == "LS-001";
    }
}