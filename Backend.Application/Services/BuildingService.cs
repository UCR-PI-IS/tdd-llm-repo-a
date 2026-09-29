using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for adding buildings.
/// </summary>
public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingService"/> class.
    /// </summary>
    /// <param name="repository">The building repository dependency.</param>
    public BuildingService(IBuildingRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Adds a new building to the system after validating duplicates and area existence.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>The added building.</returns>
    /// <exception cref="DuplicateBuildingException">Thrown when a building with the same name already exists.</exception>
    /// <exception cref="AreaNotFoundException">Thrown when the specified area does not exist.</exception>
    public async Task<Building> AddBuildingAsync(Building building)
    {
        if (await _repository.ExistsByNameAsync(building.Name))
        {
            throw new DuplicateBuildingException($"Building with name '{building.Name}' already exists");
        }

        if (!await _repository.AreaExistsAsync(building.AreaId))
        {
            throw new AreaNotFoundException();
        }

        return await _repository.AddAsync(building);
    }
}
