using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for adding buildings.
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
    /// <param name="building">The building to add.</param>
    /// <returns>The created building.</returns>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="AreaNotFoundException">Thrown when the specified area does not exist.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        if (await _buildingRepository.ExistsByNameAsync(building.Name))
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        if (!await _areaRepository.ExistsByIdAsync(building.AreaId))
        {
            throw new AreaNotFoundException($"Area with ID '{building.AreaId}' was not found.");
        }

        return await _buildingRepository.AddAsync(building);
    }
}
