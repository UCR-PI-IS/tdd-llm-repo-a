using FluentValidation.Results;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for validation errors.
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// Gets or sets the list of validation errors.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; } = new();
}
