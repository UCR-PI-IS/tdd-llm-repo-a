namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying an error message.
/// </summary>
/// <param name="Message">A human-readable description of the error.</param>
public record class ErrorResponse(string Message);
