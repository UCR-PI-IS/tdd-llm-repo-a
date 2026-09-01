namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when a business validation rule is violated.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValidationException(string message) : base(message)
    {
    }
}
