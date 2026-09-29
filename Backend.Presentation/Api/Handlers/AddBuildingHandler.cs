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
    /// <param name="request">The building creation request.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// A result indicating success (201 Created), conflict (409 Conflict), or bad request (400 Bad Request).
    /// </returns>
    public static async Task<IResult> HandleAsync(CreateBuildingDto request, IBuildingService service)
    {
        Building building;
        try
        {
            building = new Building(
                request.Name,
                request.Color,
                request.Height,
                request.Length,
                request.Width,
                request.X,
                request.Y,
                request.Z,
                request.AreaId);
        }
        catch (ArgumentException)
        {
            building = new Building(0, request.Name, request.Color, request.Height, request.Length, request.Width, request.X, request.Y, request.Z);
        }

        try
        {
            var createdBuilding = await service.AddBuildingAsync(building);

            return TypedResults.Created($"/buildings/{createdBuilding.InternalId}", new AddBuildingResponse(createdBuilding));
        }
        catch (DuplicateBuildingException ex)
        {
            return TypedResults.Conflict<string>(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest<string>(ex.Message);
        }
        catch (AreaNotFoundException ex)
        {
            return TypedResults.BadRequest<string>(ex.Message);
        }
    }
}
