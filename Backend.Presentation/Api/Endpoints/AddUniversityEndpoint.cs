using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for adding universities.
/// </summary>
public static class AddUniversityEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for adding universities.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapAddUniversityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/universities", async (
            IUniversityService service,
            AddUniversityDto dto) =>
        {
            return await AddUniversityHandler.HandleAsync(service, dto.Name, dto.Country);
        });
    }
}
