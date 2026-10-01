using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
// UpdateBuildingResponse is in the same namespace

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing building.
/// </summary>
public static class UpdateBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to update an existing building.
    /// </summary>
    /// <param name="id">The identifier of the building to update.</param>
    /// <param name="updateDto">The update data transfer object.</param>
    /// <param name="buildingService">The building service.</param>
    /// <returns>
    /// 200 OK with the success response on success,
    /// 400 Bad Request if validation fails,
    /// 404 Not Found if the building does not exist.
    /// </returns>
    public static async Task<Results<Ok<UpdateBuildingResponse>, BadRequest<string>, NotFound<string>>> HandleAsync(
        int id, UpdateBuildingDto updateDto, IBuildingService buildingService)
    {
        try
        {
            await buildingService.UpdateBuildingAsync(id, updateDto);
            return TypedResults.Ok(new UpdateBuildingResponse());
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
