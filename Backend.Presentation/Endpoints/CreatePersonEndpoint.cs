using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the person API.
/// </summary>
public static class CreatePersonEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for creating persons.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/persons", async (CreatePersonDto dto, CreatePersonHandler handler) =>
        {
            var result = await handler.HandleAsync(dto);
            return result.Result;
        }).WithName("CreatePerson");
    }
}
