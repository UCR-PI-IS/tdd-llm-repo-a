namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Represents a single validation failure for a property.
/// </summary>
public class ValidationFailure
{
    /// <summary>
    /// The name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// The error message describing the validation failure.
    /// </summary>
    public string ErrorMessage { get; set; }

    public ValidationFailure(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
