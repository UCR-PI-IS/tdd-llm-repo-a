namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for area persistence operations.
/// </summary>
public interface IAreaRepository
{
    /// <summary>
    /// Checks whether an area with the specified identifier exists.
    /// </summary>
    /// <param name="areaId">The area identifier to check.</param>
    /// <returns>True if the area exists; otherwise, false.</returns>
    Task<bool> ExistsByIdAsync(int areaId);
}
