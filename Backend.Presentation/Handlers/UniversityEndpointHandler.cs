using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Static handler for the create-university minimal API endpoint.
/// </summary>
public static class UniversityEndpointHandler
{
    /// <summary>
    /// Handles the POST /api/universities request.
    /// </summary>
    public static async Task<IResult> HandleAsync(
        CreateUniversityRequest request,
        IUniversityService service)
    {
        request ??= new CreateUniversityRequest();
        var handler = new AddUniversityHandler(service);
        var result = await handler.HandleAsync(request.Name, request.Country);
        return result.Result;
    }
}
