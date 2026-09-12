namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity with a duplicate identifier is detected.
/// </summary>
public class DuplicateIdException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateIdException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DuplicateIdException(string message) : base(message)
    {
    }
}
