using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for producing HTTP results when adding a building.
/// </summary>
public static class AddBuildingResponseFactory
{
    /// <summary>
    /// Creates a success HTTP result for a building addition.
    /// </summary>
    /// <param name="result">The persisted building entity.</param>
    /// <returns>A created result with the building response.</returns>
    public static IResult CreateSuccess(Building result)
    {
        var response = new AddBuildingResponse(result);
        return TypedResults.Created($"/api/buildings/{result.InternalId}", response);
    }

    /// <summary>
    /// Creates an error HTTP result for a building addition.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <returns>A conflict or bad request result based on the exception message.</returns>
    public static IResult CreateError(Exception ex)
    {
        if (ex.Message.Contains("already exists"))
        {
            return TypedResults.Conflict(ex.Message);
        }

        return TypedResults.BadRequest(ex.Message);
    }
}
