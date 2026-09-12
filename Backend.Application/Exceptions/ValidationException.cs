namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when validation fails for domain or application rules.
/// </summary>
public class ValidationException : ComponentCreationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    public ValidationException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public override ComponentCreationErrorType ErrorType => ComponentCreationErrorType.ValidationError;
}
