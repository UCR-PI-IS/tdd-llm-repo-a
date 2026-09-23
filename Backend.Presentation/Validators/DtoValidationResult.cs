namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Result of a DTO validation operation.
/// </summary>
public class DtoValidationResult
{
    /// <summary>
    /// Indicates whether the validation passed.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// List of validation failures. Empty when validation passes.
    /// </summary>
    public List<ValidationFailure> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DtoValidationResult"/> class.
    /// </summary>
    /// <param name="isValid">Whether the validation passed.</param>
    /// <param name="errors">List of validation failures.</param>
    public DtoValidationResult(bool isValid, List<ValidationFailure> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }
}
