using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service for managing building add operations.
/// </summary>
public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingService"/> class.
    /// </summary>
    /// <param name="buildingRepository">The building repository.</param>
    public BuildingService(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    /// <summary>
    /// Adds a new building to the system after validating uniqueness and area existence.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The created building entity.</returns>
    /// <exception cref="AreaNotFoundException">Thrown when the area does not exist.</exception>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        var areaExists = await _buildingRepository.AreaExistsAsync(building.AreaId);
        if (!areaExists)
            throw new AreaNotFoundException(building.AreaId);

        var existsByName = await _buildingRepository.ExistsByNameAsync(building.Name);
        if (existsByName)
            throw new DuplicateBuildingException(building.Name);

        return await _buildingRepository.AddAsync(building);
    }
}
