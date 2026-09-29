namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a building with the same name already exists.
/// </summary>
public class DuplicateBuildingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateBuildingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DuplicateBuildingException(string message) : base(message)
    {
    }
}
