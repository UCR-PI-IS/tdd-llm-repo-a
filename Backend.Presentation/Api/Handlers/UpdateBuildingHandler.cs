using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for updating an existing building.
/// </summary>
public static class UpdateBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to update a building.
    /// </summary>
    /// <param name="id">The internal identifier of the building to update.</param>
    /// <param name="dto">The update building DTO.</param>
    /// <param name="buildingService">The building service.</param>
    /// <returns>
    /// 200 OK with the update response on success,
    /// 400 Bad Request if validation fails,
    /// 404 Not Found if the building does not exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(int id, UpdateBuildingDto dto, IBuildingService buildingService)
    {
        try
        {
            await buildingService.UpdateBuildingAsync(id, dto);
            var response = new UpdateBuildingResponse(id, dto.Name, dto.Color);
            return TypedResults.Ok(response);
        }
        catch (BuildingNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
