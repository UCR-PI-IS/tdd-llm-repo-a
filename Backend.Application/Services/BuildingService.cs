using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for building creation.
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
    /// Adds a new building to the system after validating no duplicate exists and the area is valid.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        if (await _buildingRepository.ExistsByNameAsync(building.Name))
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        if (!await _buildingRepository.ExistsByAreaIdAsync(building.AreaId))
        {
            throw new AreaNotFoundException($"Area with id '{building.AreaId}' not found");
        }

        return await _buildingRepository.AddAsync(building);
    }
}
