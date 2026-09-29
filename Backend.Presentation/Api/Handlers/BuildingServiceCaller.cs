using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Orchestrates building creation, service invocation, and HTTP result mapping.
/// </summary>
public static class BuildingServiceCaller
{
    /// <summary>
    /// Creates a building, invokes the service add operation, and maps the result to an HTTP response.
    /// </summary>
    /// <param name="service">The building service.</param>
    /// <param name="request">The building creation request.</param>
    /// <returns>An HTTP result representing success or failure.</returns>
    public static IResult Handle(IBuildingService service, CreateBuildingDto request)
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
        var result = service.AddBuildingAsync(building).GetAwaiter().GetResult();
        return AddBuildingResponseFactory.CreateSuccess(result);
    }
}
