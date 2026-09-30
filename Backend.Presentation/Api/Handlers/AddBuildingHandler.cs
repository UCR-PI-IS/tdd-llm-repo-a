using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new building to the system.
/// </summary>
public static class AddBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a new building.
    /// </summary>
    /// <param name="buildingService">The building service dependency.</param>
    /// <param name="dto">The data transfer object containing building details.</param>
    /// <returns>A typed result indicating success or failure.</returns>
    public static async Task<Results<Created<AddBuildingResponse>, Conflict<string>, BadRequest<string>>> HandleAsync(
        IBuildingService buildingService,
        AddBuildingDto dto)
    {
        var building = CreateBuildingFromDto(dto);
        return await ExecuteAddBuildingAsync(buildingService, building);
    }

    private static Building CreateBuildingFromDto(AddBuildingDto dto)
    {
        try
        {
            return new Building(
                dto.Name, dto.Color, dto.Height, dto.Length, dto.Width,
                dto.X, dto.Y, dto.Z, dto.AreaId);
        }
        catch (ArgumentException)
        {
            return new Building(0, dto.Name, dto.Color, dto.Height, dto.Length, dto.Width, dto.X, dto.Y, dto.Z);
        }
    }

    private static async Task<Results<Created<AddBuildingResponse>, Conflict<string>, BadRequest<string>>> ExecuteAddBuildingAsync(
        IBuildingService buildingService,
        Building building)
    {
        try
        {
            var result = await buildingService.AddBuildingAsync(building);
            var response = new AddBuildingResponse(result);
            return TypedResults.Created($"/buildings/{result.InternalId}", response);
        }
        catch (DuplicateBuildingException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
