using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for building operations.
/// </summary>
public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;

    /// <summary>
    /// Initializes a new instance of the BuildingService class.
    /// </summary>
    /// <param name="buildingRepository">The building repository.</param>
    public BuildingService(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> AddBuildingAsync(string name, string color, double height, double length, double width, double x, double y, double z)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (color == null)
            throw new ArgumentNullException(nameof(color));

        // Check for duplicate building name
        var existingBuilding = await _buildingRepository.GetByNameAsync(name);
        if (existingBuilding != null)
        {
            throw new InvalidOperationException($"Building with name '{name}' already exists");
        }

        // Create new building with auto-generated internal ID
        var building = new Building(
            0, // InternalID will be set by database
            name,
            color,
            height,
            length,
            width,
            x,
            y,
            z
        );

        return await _buildingRepository.AddAsync(building);
    }
}
