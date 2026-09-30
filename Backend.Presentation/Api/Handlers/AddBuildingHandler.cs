using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Wrapper for add building handler results.
/// </summary>
public readonly struct AddBuildingResult
{
    private readonly object _result;

    /// <summary>
    /// Gets the underlying result.
    /// </summary>
    public object Result => _result;

    private AddBuildingResult(object result)
    {
        _result = result;
    }

    /// <summary>
    /// Creates a successful result with 201 Created.
    /// </summary>
    public static AddBuildingResult Success(Building building)
    {
        var location = "/api/buildings/" + building.InternalId;
        return new AddBuildingResult(TypedResults.Created(location, new AddBuildingResponse(building)));
    }

    /// <summary>
    /// Creates a conflict result with 409 status.
    /// </summary>
    public static AddBuildingResult DuplicateError()
    {
        return new AddBuildingResult(TypedResults.Conflict<string>("Building with name already exists"));
    }

    /// <summary>
    /// Creates a bad request result with 400 status.
    /// </summary>
    public static AddBuildingResult ValidationError()
    {
        return new AddBuildingResult(TypedResults.BadRequest<string>("Name cannot be empty"));
    }
}

/// <summary>
/// Handler for adding a new building.
/// </summary>
public static class AddBuildingHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a new building.
    /// </summary>
    /// <param name="request">The building creation request DTO.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// An <see cref="AddBuildingResult"/> wrapping the response,
    /// a conflict result if the building already exists,
    /// or a bad request if validation fails.
    /// </returns>
    public static async Task<AddBuildingResult> HandleAsync(CreateBuildingDto request, IBuildingService service)
    {
        try
        {
            var building = new Building(
                internalId: 0,
                name: request.Name,
                color: request.Color,
                height: request.Height,
                length: request.Length,
                width: request.Width,
                x: request.X,
                y: request.Y,
                z: request.Z);

            var result = await service.AddBuildingAsync(building, request.AreaId);
            return AddBuildingResult.Success(result);
        }
        catch (DuplicateBuildingException)
        {
            return AddBuildingResult.DuplicateError();
        }
        catch (ArgumentException)
        {
            return AddBuildingResult.ValidationError();
        }
    }
}
