using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing building.
/// </summary>
public static class UpdateBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to update a building.
    /// </summary>
    /// <param name="id">The identifier of the building to update.</param>
    /// <param name="dto">The update request containing building parameters.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated building,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="NotFound{T}"/> if the building is not found.
    /// </returns>
    public static async Task<Results<Ok<UpdateBuildingResponse>, BadRequest<string>, NotFound<string>>> HandleAsync(
        int id,
        UpdateBuildingDto dto,
        IBuildingService service)
    {
        try
        {
            var building = new Building(
                id,
                dto.Name,
                dto.Color,
                dto.Height,
                dto.Length,
                dto.Width,
                dto.X,
                dto.Y,
                dto.Z);

            await service.UpdateBuildingAsync(id, building);

            var response = new UpdateBuildingResponse(building);
            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (BuildingNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
    }
}
