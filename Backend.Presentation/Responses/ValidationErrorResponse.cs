namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying validation error details.
/// </summary>
/// <param name="Errors">The list of validation error messages.</param>
public record class ValidationErrorResponse(List<string> Errors);
