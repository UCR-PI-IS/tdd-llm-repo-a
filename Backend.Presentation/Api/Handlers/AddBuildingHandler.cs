using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Wrapper for add building handler results that implements <see cref="IResult"/>
/// to remain compatible with Minimal APIs while reducing class coupling.
/// </summary>
public readonly struct AddBuildingResult
{
    /// <summary>
    /// Gets the underlying result.
    /// </summary>
    public IResult Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBuildingResult"/> struct.
    /// </summary>
    /// <param name="result">The underlying ASP.NET Core result.</param>
    public AddBuildingResult(IResult result)
    {
        Result = result;
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
    /// <param name="request">The add building request.</param>
    /// <param name="service">The building service.</param>
    /// <returns>An <see cref="AddBuildingResult"/> wrapping the appropriate HTTP response.</returns>
    public static async Task<AddBuildingResult> HandleAsync(AddBuildingRequest request, IBuildingService service)
    {
        try
        {
            var building = new Building(
                0, request.Name ?? string.Empty, request.Color ?? string.Empty,
                request.Height, request.Length, request.Width,
                request.X, request.Y, request.Z, request.AreaId);

            var created = await service.AddBuildingAsync(building);
            var response = new AddBuildingResponse(created);
            return new AddBuildingResult(TypedResults.Created($"/buildings/{created.InternalId}", response));
        }
        catch (DuplicateBuildingException ex)
        {
            return new AddBuildingResult(TypedResults.Conflict(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return new AddBuildingResult(TypedResults.BadRequest(ex.Message));
        }
    }
}
