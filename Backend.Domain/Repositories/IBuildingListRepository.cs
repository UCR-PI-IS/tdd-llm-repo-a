using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for accessing building data sources.
/// </summary>
public interface IBuildingListRepository
{
    /// <summary>
    /// Retrieves all buildings.
    /// </summary>
    Task<List<Building>> GetAllBuildingsAsync();
}
