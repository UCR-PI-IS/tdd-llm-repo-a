namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a referenced area does not exist.
/// </summary>
public class AreaNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AreaNotFoundException"/> class.
    /// </summary>
    /// <param name="areaId">The identifier of the area that was not found.</param>
    public AreaNotFoundException(int areaId)
        : base($"Area with id '{areaId}' was not found")
    {
    }
}
