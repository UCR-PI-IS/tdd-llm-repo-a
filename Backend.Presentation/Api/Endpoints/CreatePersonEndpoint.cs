using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for creating persons.
/// </summary>
public static class CreatePersonEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for creating persons.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/persons", async (
            [FromServices] IPersonService service,
            [FromBody] CreatePersonDto dto) =>
        {
            return await CreatePersonHandler.HandleAsync(service, dto);
        });
    }
}
