using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for creating universities.
/// </summary>
public static class CreateUniversityEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for creating universities.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapCreateUniversityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/universities", async (
            IUniversityService service,
            CreateUniversityDto dto) =>
        {
            var handler = new AddUniversityHandler(service);
            return await handler.HandleAsync(dto.Name, dto.Country);
        });
    }
}
