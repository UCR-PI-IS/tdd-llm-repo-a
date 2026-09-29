namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when an area is not found.
/// </summary>
public class AreaNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AreaNotFoundException"/> class.
    /// </summary>
    public AreaNotFoundException() : base("Area not found.")
    {
    }
}
