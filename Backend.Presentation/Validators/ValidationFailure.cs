namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Represents a validation failure for a specific property.
/// </summary>
public class ValidationFailure
{
    /// <summary>
    /// Name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Error message describing the validation failure.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationFailure"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property that failed validation.</param>
    /// <param name="errorMessage">Error message describing the validation failure.</param>
    public ValidationFailure(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
