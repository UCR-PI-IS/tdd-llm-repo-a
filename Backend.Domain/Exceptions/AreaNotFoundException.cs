namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when an area is not found.
/// </summary>
public class AreaNotFoundException : BuildingDomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AreaNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AreaNotFoundException(string message) : base(message)
    {
    }
}
