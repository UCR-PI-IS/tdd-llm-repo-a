namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when an attempt is made to create an entity with an ID that already exists.
/// </summary>
public class DuplicateIdException : ComponentCreationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateIdException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DuplicateIdException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public override ComponentCreationErrorType ErrorType => ComponentCreationErrorType.DuplicateIdError;
}
