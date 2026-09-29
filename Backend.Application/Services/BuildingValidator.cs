using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Encapsulates building validation rules used by <see cref="BuildingService"/>.
/// </summary>
internal static class BuildingValidator
{
    /// <summary>
    /// Validates that the specified area exists.
    /// </summary>
    /// <param name="areaId">The area identifier.</param>
    /// <param name="areaRepository">The area repository.</param>
    public static async Task ValidateAreaExistsAsync(int areaId, IAreaRepository areaRepository)
    {
        if (!await areaRepository.ExistsAsync(areaId))
        {
            throw new AreaNotFoundException($"Area with id {areaId} was not found.");
        }
    }

    /// <summary>
    /// Validates that no building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name.</param>
    /// <param name="buildingRepository">The building repository.</param>
    public static async Task ValidateNameNotExistsAsync(string name, IBuildingRepository buildingRepository)
    {
        if (await buildingRepository.ExistsByNameAsync(name))
        {
            throw new DuplicateBuildingException($"Building with name '{name}' already exists");
        }
    }
}
