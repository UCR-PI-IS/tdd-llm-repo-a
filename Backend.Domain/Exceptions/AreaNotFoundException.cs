namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to add a building to a non-existent area.
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
