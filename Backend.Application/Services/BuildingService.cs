using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Validation;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for adding buildings.
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

    /// <inheritdoc />
    public async Task<Building> AddBuildingAsync(Building building)
    {
        Guard.AgainstNullOrEmpty(building.Name, nameof(building.Name));
        Guard.AgainstNullOrEmpty(building.Color, nameof(building.Color));

        var exists = await _buildingRepository.ExistsByNameAsync(building.Name);
        if (exists)
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        var areaExists = await _buildingRepository.AreaExistsAsync(building.AreaId);
        if (!areaExists)
        {
            throw new AreaNotFoundException($"Area with id '{building.AreaId}' not found");
        }

        return await _buildingRepository.AddAsync(building);
    }

    /// <inheritdoc />
    public async Task UpdateBuildingAsync(int id, Building building)
    {
        var existingBuilding = await _buildingRepository.GetByIdAsync(id);
        if (existingBuilding == null)
        {
            throw new BuildingNotFoundException(id);
        }

        Guard.AgainstNullOrEmpty(building.Name, "name");
        Guard.AgainstNullOrEmpty(building.Color, "color");
        Guard.AgainstNonPositive(building.Height, "height");
        Guard.AgainstNonPositive(building.Length, "length");
        Guard.AgainstNonPositive(building.Width, "width");

        var buildingToUpdate = new Building(
            id,
            building.Name,
            building.Color,
            building.Height,
            building.Length,
            building.Width,
            building.X,
            building.Y,
            building.Z,
            building.AreaId);

        await _buildingRepository.UpdateAsync(buildingToUpdate);
    }
}
