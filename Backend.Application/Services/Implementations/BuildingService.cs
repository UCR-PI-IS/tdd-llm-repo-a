using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service for managing building creation operations.
/// </summary>
public class BuildingService : IBuildingCreateService
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
    /// Adds a new building to the system after checking for duplicates and valid area.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="AreaNotFoundException">Thrown when the referenced area does not exist.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        var exists = await _repository.ExistsByNameAsync(building.Name);
        if (exists)
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");

        var areaExists = await _repository.AreaExistsAsync(building.AreaId);
        if (!areaExists)
            throw new AreaNotFoundException($"Area with id '{building.AreaId}' not found");

        return await _repository.AddAsync(building);
    }
}
