using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Represents an error response.
/// </summary>
public record class ErrorResponse(String Message)
{
    /// <summary>
    /// Creates an appropriate HTTP result from the given exception.
    /// </summary>
    /// <param name="ex">The exception to convert.</param>
    /// <returns>A <see cref="BadRequest{ErrorResponse}"/> or <see cref="NotFound{ErrorResponse}"/> result.</returns>
    public static object FromException(Exception ex)
    {
        if (ex is ArgumentException ae && ae.ParamName == "learningSpaceId")
            return TypedResults.BadRequest(new ErrorResponse(ae.Message));
        if (ex is KeyNotFoundException ke)
            return TypedResults.NotFound(new ErrorResponse(ke.Message));
        throw ex;
    }
}
