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
        // Validate building properties
        if (string.IsNullOrEmpty(building.Name))
            throw new ArgumentException("Name cannot be empty", nameof(building.Name));

        if (string.IsNullOrEmpty(building.Color))
            throw new ArgumentException("Color cannot be empty", nameof(building.Color));

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
        var building = await _buildingRepository.GetByIdAsync(id);
        if (building is null)
        {
            throw new BuildingNotFoundException(id);
        }

        building.Update(
            updateDto.Name,
            updateDto.Color,
            updateDto.Height,
            updateDto.Length,
            updateDto.Width,
            updateDto.X,
            updateDto.Y,
            updateDto.Z);

        await _buildingRepository.UpdateAsync(building);
    }
}
