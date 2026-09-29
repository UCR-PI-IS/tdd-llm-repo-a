using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new building.
/// </summary>
public static class AddBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a new building.
    /// </summary>
    /// <param name="request">The data transfer object containing the building parameters.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="AddBuildingResult"/> wrapping the created building,
    /// a bad request if validation fails,
    /// a conflict if the building already exists,
    /// or a not found if the area does not exist.
    /// </returns>
    public static async Task<AddBuildingResult> HandleAsync(AddBuildingDto request, IBuildingService service)
    {
        try
        {
            var result = await service.AddBuildingAsync(new Building(
                request.Name,
                request.Color,
                request.Height,
                request.Length,
                request.Width,
                request.X,
                request.Y,
                request.Z,
                request.AreaId));

            return AddBuildingResultFactory.Success(result);
        }
        catch (Exception ex)
        {
            return AddBuildingResultFactory.FromException(ex);
        }
    }
}
