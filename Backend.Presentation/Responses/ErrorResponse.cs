namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Represents an error response with a message.
/// </summary>
/// <param name="Message">The error message.</param>
public record class ErrorResponse(string Message);
