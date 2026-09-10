namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when input validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message describing the validation failure.</param>
    public ValidationException(string message) : base(message)
    {
    }
}
