namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a building with the specified ID is not found.
/// </summary>
public class BuildingNotFoundException : Exception
{
    /// <summary>
    /// Gets the ID of the building that was not found.
    /// </summary>
    public int BuildingId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingNotFoundException"/> class.
    /// </summary>
    /// <param name="buildingId">The ID of the building that was not found.</param>
    public BuildingNotFoundException(int buildingId)
        : base($"Building with ID '{buildingId}' was not found.")
    {
        BuildingId = buildingId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingNotFoundException"/> class with a custom message.
    /// </summary>
    /// <param name="buildingId">The ID of the building that was not found.</param>
    /// <param name="message">The custom error message.</param>
    public BuildingNotFoundException(int buildingId, string message)
        : base(message)
    {
        BuildingId = buildingId;
    }
}
