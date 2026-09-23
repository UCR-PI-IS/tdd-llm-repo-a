using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the person creation API.
/// </summary>
public static class CreatePersonEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for creating persons.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> used to map the endpoint.</param>
    public static void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/persons", (CreatePersonHandler handler, CreatePersonDto dto) => handler.HandleAsync(dto))
            .WithName("CreatePerson")
            .WithOpenApi();
    }
}
