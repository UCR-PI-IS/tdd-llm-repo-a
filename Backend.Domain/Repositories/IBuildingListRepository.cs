using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for accessing building data sources.
/// </summary>
public interface IBuildingListRepository
{
    /// <summary>
    /// Retrieves all buildings from the data source.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="Building"/> entities.</returns>
    Task<List<Building>> GetAllBuildingsAsync();
}
