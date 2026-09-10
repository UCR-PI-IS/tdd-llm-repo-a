namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when a duplicate component ID is detected.
/// </summary>
public class DuplicateIdException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateIdException"/> class.
    /// </summary>
    /// <param name="message">The error message describing the duplicate ID.</param>
    public DuplicateIdException(string message) : base(message)
    {
    }
}
