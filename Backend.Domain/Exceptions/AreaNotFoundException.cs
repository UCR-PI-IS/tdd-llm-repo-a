namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when the specified area does not exist.
/// </summary>
public class AreaNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AreaNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AreaNotFoundException(string message) : base(message)
    {
    }
}
