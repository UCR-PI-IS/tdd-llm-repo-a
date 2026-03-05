namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service interface for building operations.
/// </summary>
public interface IBuildingService
{
    /// <summary>
    /// Adds a new building to the system.
    /// </summary>
    /// <param name="name">Name of the building.</param>
    /// <param name="color">Color of the building.</param>
    /// <param name="height">Height of the building.</param>
    /// <param name="length">Length of the building.</param>
    /// <param name="width">Width of the building.</param>
    /// <param name="x">X coordinate of the building.</param>
    /// <param name="y">Y coordinate of the building.</param>
    /// <param name="z">Z coordinate of the building.</param>
    /// <returns>True if addition is successful, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="ArgumentNullException">Thrown when required fields are null.</exception>
    Task<bool> AddBuildingAsync(string name, string color, double height, double length, double width, double x, double y, double z);
}
