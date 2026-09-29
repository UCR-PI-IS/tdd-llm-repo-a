using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new building.
/// </summary>
public static class AddBuildingHandler
{
    /// <summary>
    /// Handles the request to add a new building.
    /// </summary>
    /// <param name="service">The building service.</param>
    /// <param name="request">The building creation request.</param>
    /// <returns>
    /// A created result when successful,
    /// a conflict result when a duplicate exists,
    /// or a bad request result when validation fails.
    /// </returns>
    public static Task<IResult> HandleAsync(IBuildingService service, CreateBuildingDto request)
    {
        if (request == null)
        {
            request = new CreateBuildingDto();
        }

        try
        {
            return Task.FromResult(BuildingServiceCaller.Handle(service, request));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AddBuildingResponseFactory.CreateError(ex));
        }
    }
}
