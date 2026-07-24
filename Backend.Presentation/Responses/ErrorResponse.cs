namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying an error message.
/// </summary>
/// <param name="Message">A human-readable error message.</param>
public record class ErrorResponse(string Message);
