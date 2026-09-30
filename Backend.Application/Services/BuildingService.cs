using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service for managing building operations.
/// </summary>
public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IAreaRepository _areaRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingService"/> class.
    /// </summary>
    /// <param name="buildingRepository">The building repository.</param>
    /// <param name="areaRepository">The area repository.</param>
    public BuildingService(IBuildingRepository buildingRepository, IAreaRepository areaRepository)
    {
        _buildingRepository = buildingRepository;
        _areaRepository = areaRepository;
    }

    /// <summary>
    /// Adds a new building to the system.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <param name="areaId">The ID of the area where the building is located.</param>
    /// <returns>The added building entity.</returns>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="AreaNotFoundException">Thrown when the specified area does not exist.</exception>
    public async Task<Building> AddBuildingAsync(Building building, int areaId)
    {
        // Check for duplicate building name
        var exists = await _buildingRepository.ExistsByNameAsync(building.Name);
        if (exists)
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        // Check if area exists
        var areaExists = await _areaRepository.ExistsAsync(areaId);
        if (!areaExists)
        {
            throw new AreaNotFoundException($"Area with ID '{areaId}' not found");
        }

        // Add the building
        return await _buildingRepository.AddAsync(building);
    }
}
