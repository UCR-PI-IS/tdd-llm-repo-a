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
    public async Task UpdateBuildingAsync(int id, UpdateBuildingDto updateDto)
    {
        var existingBuilding = await _buildingRepository.GetByIdAsync(id);
        if (existingBuilding == null)
        {
            throw new BuildingNotFoundException(id);
        }

        existingBuilding.Update(
            updateDto.Name,
            updateDto.Color,
            updateDto.Height,
            updateDto.Length,
            updateDto.Width,
            updateDto.X,
            updateDto.Y,
            updateDto.Z);

        await _buildingRepository.UpdateAsync(existingBuilding);
    }
}
