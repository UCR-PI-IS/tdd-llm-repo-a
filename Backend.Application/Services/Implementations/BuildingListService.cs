using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving building listings.
/// </summary>
internal class BuildingListService : IBuildingListService
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
    /// Retrieves all buildings from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="Building"/> entities.</returns>
    public Task<List<Building>> GetAllBuildingsAsync()
    {
        return _buildingListRepository.GetAllBuildingsAsync();
    }
}
