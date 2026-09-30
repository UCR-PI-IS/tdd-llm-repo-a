using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new building.
/// </summary>
public static class AddBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a new building.
    /// </summary>
    /// <param name="request">The add building request containing building data.</param>
    /// <param name="buildingService">The building service for business logic.</param>
    /// <returns>An <see cref="IResult"/> response indicating success or failure.</returns>
    public static async Task<IResult> HandleAsync(AddBuildingRequest request, IBuildingService buildingService)
    {
        try
        {
            var building = BuildingMapper.ToEntity(request);
            var addedBuilding = await buildingService.AddBuildingAsync(building);
            var response = BuildingMapper.ToAddResponse(addedBuilding);
            return Results.Created($"/buildings/{addedBuilding.InternalId}", response);
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
