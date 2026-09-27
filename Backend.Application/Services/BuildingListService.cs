using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for retrieving building list data.
/// </summary>
public class BuildingListService : IBuildingListService
{
    private readonly IBuildingListRepository _buildingListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingListService"/> class.
    /// </summary>
    /// <param name="buildingListRepository">The building list repository dependency.</param>
    public BuildingListService(IBuildingListRepository buildingListRepository)
    {
        _buildingListRepository = buildingListRepository;
    }

    /// <summary>
    /// Retrieves a list of all buildings available in the database.
    /// </summary>
    /// <returns>A list of building entities.</returns>
    public Task<List<Building>> GetAllBuildingsAsync()
    {
        return _buildingListRepository.GetAllBuildingsAsync();
    }
}
