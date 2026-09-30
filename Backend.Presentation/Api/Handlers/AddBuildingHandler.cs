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
public readonly struct AddBuildingResult : IResult
{
    private readonly IResult _result;

    /// <summary>
    /// Gets the underlying result.
    /// </summary>
    public IResult Result => _result;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBuildingResult"/> struct.
    /// </summary>
    /// <param name="result">The underlying ASP.NET Core result.</param>
    public AddBuildingResult(IResult result)
    {
        _result = result;
    }

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
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
    /// <param name="service">The building creation service.</param>
    /// <param name="dto">The data transfer object containing building information.</param>
    /// <returns>
    /// An <see cref="AddBuildingResult"/> wrapping a Created response on success,
    /// a Conflict if the building already exists, or a BadRequest if validation fails.
    /// </returns>
    public static async Task<AddBuildingResult> HandleAsync(IBuildingCreateService service, AddBuildingDto dto)
    {
        try
        {
            var building = new Building(
                dto.Name, dto.Color, dto.Height, dto.Length, dto.Width,
                dto.X, dto.Y, dto.Z, dto.AreaId);

            var created = await service.AddBuildingAsync(building);
            return new AddBuildingResult(
                TypedResults.Created($"/buildings/{created.InternalId}", new AddBuildingResponse(created)));
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
