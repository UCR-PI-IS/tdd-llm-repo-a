namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for area repository operations.
/// </summary>
public interface IAreaRepository
{
    /// <summary>
    /// Checks if an area with the specified ID exists.
    /// </summary>
    /// <param name="areaId">The area ID to check.</param>
    /// <returns>True if the area exists, otherwise false.</returns>
    Task<bool> ExistsAsync(int areaId);
}
