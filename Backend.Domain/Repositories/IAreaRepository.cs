namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for area existence checks.
/// </summary>
public interface IAreaRepository
{
    /// <summary>
    /// Checks whether an area with the specified identifier exists.
    /// </summary>
    /// <param name="areaId">The area identifier.</param>
    /// <returns>True if the area exists; otherwise false.</returns>
    Task<bool> ExistsAsync(int areaId);
}
