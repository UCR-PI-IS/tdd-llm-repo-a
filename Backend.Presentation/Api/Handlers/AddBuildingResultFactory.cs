using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating <see cref="AddBuildingResult"/> instances.
/// Centralizes response construction to keep the wrapper struct simple.
/// </summary>
internal static class AddBuildingResultFactory
{
    /// <summary>
    /// Creates a success result for the building creation.
    /// </summary>
    public static AddBuildingResult Success(Building building)
    {
        return new AddBuildingResult(
            Results.Created($"/api/buildings/{building.InternalId}", new AddBuildingResponse(building)));
    }

    /// <summary>
    /// Creates a result from an exception using its type name.
    /// </summary>
    public static AddBuildingResult FromException(Exception ex)
    {
        if (ex.GetType().Name == "DuplicateBuildingException")
        {
            return Conflict(ex);
        }

        return BadRequest(ex);
    }

    /// <summary>
    /// Creates a conflict result from the exception.
    /// </summary>
    public static AddBuildingResult Conflict(Exception ex)
    {
        return new AddBuildingResult(Results.Conflict(ex.Message));
    }

    /// <summary>
    /// Creates a bad request result from the exception.
    /// </summary>
    public static AddBuildingResult BadRequest(Exception ex)
    {
        return new AddBuildingResult(Results.BadRequest(ex.Message));
    }
}
