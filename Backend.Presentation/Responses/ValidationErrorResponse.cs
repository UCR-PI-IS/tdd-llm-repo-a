using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying validation error details.
/// </summary>
/// <param name="Errors">List of validation failures.</param>
public record class ValidationErrorResponse(List<ValidationFailure> Errors);
