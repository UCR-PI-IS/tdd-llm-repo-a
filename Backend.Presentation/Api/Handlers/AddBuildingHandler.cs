using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new building.
/// </summary>
public static class AddBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a new building.
    /// </summary>
    /// <param name="request">The building creation request.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="IResult"/> representing the operation outcome.
    /// </returns>
    public static async Task<IResult> HandleAsync(AddBuildingRequest request, IBuildingService service)
    {
        try
        {
            var building = new Building(
                request.Name,
                request.Color,
                request.Height,
                request.Length,
                request.Width,
                request.X,
                request.Y,
                request.Z,
                request.AreaId);

            var result = await service.AddBuildingAsync(building);

            var response = AddBuildingResponse.FromBuilding(result);

            return Results.Created($"/api/buildings/{result.InternalId}", response);
        }
        catch (DuplicateBuildingException ex)
        {
            return Results.Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
