using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service for managing building operations.
/// </summary>
public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingService"/> class.
    /// </summary>
    /// <param name="repository">The building repository.</param>
    public BuildingService(IBuildingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Adds a new building to the system.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="AreaNotFoundException">Thrown when the specified area does not exist.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        var exists = await _repository.ExistsByNameAsync(building.Name);
        if (exists)
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        var areaExists = await _repository.AreaExistsAsync(building.AreaId);
        if (!areaExists)
        {
            throw new AreaNotFoundException($"Area with ID '{building.AreaId}' does not exist");
        }

        return await _repository.AddAsync(building);
    }
}
