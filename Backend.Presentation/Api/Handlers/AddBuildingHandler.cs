using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    /// <param name="request">The add building request DTO.</param>
    /// <param name="buildingService">The building service.</param>
    /// <returns>
    /// 201 Created with the building data on success,
    /// 409 Conflict if the building already exists,
    /// 400 Bad Request if validation fails.
    /// </returns>
    public static async Task<IResult> HandleAsync(AddBuildingRequest request, IBuildingService buildingService)
    {
        try
        {
            var building = new Building(
                0, request.Name, request.Color, request.Height, request.Length, request.Width,
                request.X, request.Y, request.Z, request.AreaId);

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
