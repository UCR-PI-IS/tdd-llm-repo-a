namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for validation errors.
/// </summary>
public record class ValidationErrorResponse(
    string Message,
    List<string> Errors);
