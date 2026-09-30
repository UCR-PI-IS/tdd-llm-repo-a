namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to add a building with a name that already exists.
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
