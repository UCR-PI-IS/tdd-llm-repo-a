using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Request object carrying the parameters needed to update a building.
/// </summary>
/// <param name="Name">New name of the building.</param>
/// <param name="Color">New color of the building.</param>
/// <param name="Height">New height of the building in meters.</param>
/// <param name="Length">New length of the building in meters.</param>
/// <param name="Width">New width of the building in meters.</param>
/// <param name="X">New X coordinate.</param>
/// <param name="Y">New Y coordinate.</param>
/// <param name="Z">New Z coordinate.</param>
public record class UpdateBuildingRequest(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z);

/// <summary>
/// Response object carrying the updated building data.
/// </summary>
/// <param name="Id">Unique identifier of the building.</param>
/// <param name="Name">Name of the building.</param>
/// <param name="Color">Color of the building.</param>
/// <param name="Height">Height of the building in meters.</param>
/// <param name="Length">Length of the building in meters.</param>
/// <param name="Width">Width of the building in meters.</param>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
/// <param name="Z">Z coordinate.</param>
public record class UpdateBuildingResponse(
    int Id,
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
    /// <param name="id">The unique identifier of the building to update.</param>
    /// <param name="request">The update request containing building parameters.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the updated building,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="NotFound{T}"/> if the building is not found.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        int id,
        UpdateBuildingRequest request,
        IBuildingService service)
    {
        try
        {
            var dto = new UpdateBuildingDto(
                request.Name,
                request.Color,
                request.Height,
                request.Length,
                request.Width,
                request.X,
                request.Y,
                request.Z);

            await service.UpdateBuildingAsync(id, dto);

            var response = new UpdateBuildingResponse(
                id,
                request.Name,
                request.Color,
                request.Height,
                request.Length,
                request.Width,
                request.X,
                request.Y,
                request.Z);

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
