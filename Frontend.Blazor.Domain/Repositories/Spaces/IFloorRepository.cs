using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.Spaces;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.Spaces;

/// <summary>
/// Defines the contract for the Floor repository, which provides methods to manage and persist
/// <see cref="Floor"/> entities in the system.
/// </summary>
public interface IFloorRepository
{
    /// <summary>
    /// Adds a new floor to a building.
    /// </summary>
    /// <param name="buildingId">The internal Id of a building</param>
    /// <returns>A <see cref="Task"/> result that is true if the floor was successfully created;
    /// otherwise, false. </returns>
    public Task<bool> CreateFloorAsync(int buildingId);

    /// <summary>
    /// List all floors available in a building.
    /// </summary>
    /// <param name="buildingId">The internal Id of the building.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="Floor"/>
    /// entities in the specified building, or <c>null</c> if none are found.
    /// </returns>
    public Task<List<Floor>?> ListFloorsAsync(int buildingId);

    /// <summary>
    /// Deletes a specific floor in a building.
    /// </summary>
    /// <param name="floorId">The internal Id of the floor entity</param>
    /// <returns>
    /// A <see cref="Task"/> result that is true if the floor was successfully deleted;
    /// otherwise, false.
    /// </returns>
    public Task<bool> DeleteFloorAsync(int floorId);
}
