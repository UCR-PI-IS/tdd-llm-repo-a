using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Response object for a successfully updated building.
/// </summary>
/// <param name="Message">Confirmation message.</param>
public record class UpdateBuildingResponse(string Message);

/// <summary>
/// Handler for updating an existing building.
/// </summary>
public static class UpdateBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to update a building.
    /// </summary>
    /// <param name="id">The identifier of the building to update.</param>
    /// <param name="dto">The update data transfer object.</param>
    /// <param name="buildingService">The building service.</param>
    /// <returns>
    /// 200 OK with success response on success,
    /// 400 Bad Request if validation fails,
    /// 404 Not Found if the building does not exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        int id,
        UpdateBuildingDto dto,
        IBuildingService buildingService)
    {
        try
        {
            await buildingService.UpdateBuildingAsync(id, dto);
            var response = new UpdateBuildingResponse("Building updated successfully.");
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
