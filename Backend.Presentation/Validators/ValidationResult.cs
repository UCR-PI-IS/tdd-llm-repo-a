namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether validation succeeded.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets the collection of validation failures.
    /// </summary>
    public List<ValidationFailure> Errors { get; }

    public ValidationResult(List<ValidationFailure> errors)
    {
        Errors = errors;
    }
}
