using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
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
    /// Adds a new building to the system after validating no duplicates exist and the area is valid.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="AreaNotFoundException">Thrown when the specified area does not exist.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        // Validate building properties
        if (string.IsNullOrEmpty(building.Name))
            throw new ArgumentException("Name cannot be empty", nameof(building.Name));

        if (string.IsNullOrEmpty(building.Color))
            throw new ArgumentException("Color cannot be empty", nameof(building.Color));

        // Check for duplicate building name
        bool exists = await _buildingRepository.ExistsByNameAsync(building.Name);
        if (exists)
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        // Check if area exists
        bool areaExists = await _buildingRepository.AreaExistsAsync(building.AreaId);
        if (!areaExists)
        {
            throw new AreaNotFoundException($"Area with ID {building.AreaId} does not exist");
        }

        // Add the building
        return await _buildingRepository.AddAsync(building);
    }
}
