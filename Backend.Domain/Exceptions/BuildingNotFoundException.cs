namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a building with the specified identifier is not found.
/// </summary>
public class BuildingNotFoundException : Exception
{
    /// <summary>
    /// Gets the identifier of the building that was not found.
    /// </summary>
    public int BuildingId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingNotFoundException"/> class.
    /// </summary>
    /// <param name="buildingId">The identifier of the building that was not found.</param>
    public BuildingNotFoundException(int buildingId)
        : base($"Building with id '{buildingId}' was not found.")
    {
        BuildingId = buildingId;
    }
}
