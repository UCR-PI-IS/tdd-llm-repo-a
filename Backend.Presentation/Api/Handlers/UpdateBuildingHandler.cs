using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Response object carrying the updated building data.
/// </summary>
/// <param name="InternalId">Unique identifier for the building.</param>
/// <param name="Name">Name of the building.</param>
/// <param name="Color">Color of the building.</param>
/// <param name="Height">Height of the building in meters.</param>
/// <param name="Length">Length of the building in meters.</param>
/// <param name="Width">Width of the building in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
public record class UpdateBuildingResponse(
    int InternalId,
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z);

/// <summary>
/// Handler for updating an existing building.
/// </summary>
public static class UpdateBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to update a building.
    /// </summary>
    /// <param name="buildingId">The identifier of the building to update.</param>
    /// <param name="dto">The update data transfer object.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated building,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="NotFound{T}"/> if the building is not found.
    /// </returns>
    public static async Task<Results<Ok<UpdateBuildingResponse>, BadRequest<string>, NotFound<string>>> HandleAsync(
        int buildingId,
        UpdateBuildingDto dto,
        IBuildingService service)
    {
        try
        {
            await service.UpdateBuildingAsync(buildingId, dto);

            var response = new UpdateBuildingResponse(
                buildingId,
                dto.Name,
                dto.Color,
                dto.Height,
                dto.Length,
                dto.Width,
                dto.X,
                dto.Y,
                dto.Z);

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
