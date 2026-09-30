using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for building operations.
/// </summary>
public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingService"/> class.
    /// </summary>
    /// <param name="buildingRepository">The building repository dependency.</param>
    public BuildingService(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    /// <summary>
    /// Adds a new building after checking for duplicates and valid area.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    /// <exception cref="AreaNotFoundException">Thrown when the area does not exist.</exception>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        var areaExists = await _buildingRepository.AreaExistsAsync(building.AreaId);
        if (!areaExists)
        {
            throw new AreaNotFoundException($"Area with id '{building.AreaId}' does not exist");
        }

        var exists = await _buildingRepository.ExistsByNameAsync(building.Name);
        if (exists)
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        return await _buildingRepository.AddAsync(building);
    }
}
