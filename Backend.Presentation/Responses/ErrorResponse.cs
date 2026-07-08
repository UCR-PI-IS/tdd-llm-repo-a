using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Error response for API errors.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
    /// </summary>
    /// <param name="message">The error message</param>
    public ErrorResponse(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates a <see cref="BadRequest{ErrorResponse}"/> result.
    /// </summary>
    public static IResult BadRequestResult(string message) =>
        Results.BadRequest(new ErrorResponse(message));

    /// <summary>
    /// Creates a <see cref="NotFound{ErrorResponse}"/> result.
    /// </summary>
    public static IResult NotFoundResult(string message) =>
        Results.NotFound(new ErrorResponse(message));
}
