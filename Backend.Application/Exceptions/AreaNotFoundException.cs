namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to add a building to a non-existent area.
/// </summary>
public class AreaNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AreaNotFoundException"/> class.
    /// </summary>
    /// <param name="areaId">The area identifier that was not found.</param>
    public AreaNotFoundException(int areaId)
        : base($"Area with id '{areaId}' was not found")
    {
    }
}
