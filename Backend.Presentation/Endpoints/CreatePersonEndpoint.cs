using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
    public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/persons", CreatePersonHandler.HandleAsync)
            .WithName("CreatePerson")
            .WithOpenApi();

        return builder;
    }
}
