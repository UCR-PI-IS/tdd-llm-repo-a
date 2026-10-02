using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages building creation.
/// </summary>
public interface IBuildingService
{
    /// <summary>
    /// Adds a new building to the system after validating business rules.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    Task<Building> AddBuildingAsync(Building building);

    /// <summary>
    /// Updates an existing building with the provided data.
    /// </summary>
    /// <param name="id">The internal identifier of the building to update.</param>
    /// <param name="updateDto">The data to apply to the building.</param>
    Task UpdateBuildingAsync(int id, UpdateBuildingDto updateDto);
}
