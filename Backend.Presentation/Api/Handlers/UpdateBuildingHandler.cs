using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Response object for a successfully updated building.
/// </summary>
/// <param name="InternalId">The internal identifier of the building.</param>
/// <param name="Name">Name of the building.</param>
/// <param name="Color">Color of the building.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Length">Length in meters.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
/// <param name="Z">Z coordinate.</param>
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
    /// <param name="buildingId">The internal identifier of the building to update.</param>
    /// <param name="updateDto">The data transfer object containing the updated values.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated building,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="NotFound{T}"/> if the building does not exist.
    /// </returns>
    public static async Task<Results<Ok<UpdateBuildingResponse>, BadRequest<string>, NotFound<string>>> HandleAsync(
        int buildingId,
        UpdateBuildingDto updateDto,
        IBuildingService service)
    {
        try
        {
            await service.UpdateBuildingAsync(buildingId, updateDto);

            var response = new UpdateBuildingResponse(
                buildingId,
                updateDto.Name,
                updateDto.Color,
                updateDto.Height,
                updateDto.Length,
                updateDto.Width,
                updateDto.X,
                updateDto.Y,
                updateDto.Z);

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
