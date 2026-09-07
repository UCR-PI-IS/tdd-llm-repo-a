namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business validation rule is violated.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ValidationException(string message) : base(message)
    {
    }
}
