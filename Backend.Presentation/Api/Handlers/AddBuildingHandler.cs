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
    /// Handles the asynchronous request to add a new building.
    /// </summary>
    /// <param name="request">The add building request DTO.</param>
    /// <param name="service">The building service.</param>
    /// <returns>
    /// 201 Created with the building data on success,
    /// 409 Conflict if the building already exists,
    /// 400 Bad Request if validation fails.
    /// </returns>
    public static Task<IResult> HandleAsync(AddBuildingRequest request, IBuildingService service)
    {
        return AddBuildingResponseFactory.CreateResponseAsync(request, service);
    }
}
