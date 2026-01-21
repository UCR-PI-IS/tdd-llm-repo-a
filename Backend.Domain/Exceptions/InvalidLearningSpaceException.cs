namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a learning space id or entity is invalid.
/// </summary>
public class InvalidLearningSpaceException : System.Exception
{
    public InvalidLearningSpaceException(string message) : base(message) { }
}
