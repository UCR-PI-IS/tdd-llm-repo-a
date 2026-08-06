namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Represents an error response returned by the API.
/// </summary>
/// <param name="Message">The error message.</param>
public record class ErrorResponse(string Message);
