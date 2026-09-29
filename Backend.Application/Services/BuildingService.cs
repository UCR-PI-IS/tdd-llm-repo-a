using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
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
    /// Adds a new building to the system after validating area existence and uniqueness.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        await BuildingValidator.ValidateAreaExistsAsync(building.AreaId, _areaRepository);
        await BuildingValidator.ValidateNameNotExistsAsync(building.Name, _buildingRepository);
        await _buildingRepository.AddAsync(building);
        return building;
    }
}
