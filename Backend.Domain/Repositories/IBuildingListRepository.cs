using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for retrieving building data.
/// </summary>
public interface IBuildingListRepository
{
    /// <summary>
    /// Retrieves all buildings from the data store.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="Building"/> entities.</returns>
    Task<List<Building>> GetAllBuildingsAsync();
}
