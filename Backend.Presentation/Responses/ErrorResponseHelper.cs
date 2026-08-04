using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Helper class for creating error responses.
/// </summary>
public static class ErrorResponseHelper
{
    /// <summary>
    /// Creates a bad request response with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A BadRequest result with an ErrorResponse.</returns>
    public static BadRequest<ErrorResponse> BadRequest(string message)
    {
        return TypedResults.BadRequest(new ErrorResponse(message));
    }

    /// <summary>
    /// Creates a not found response with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A NotFound result with an ErrorResponse.</returns>
    public static NotFound<ErrorResponse> NotFound(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }
}
