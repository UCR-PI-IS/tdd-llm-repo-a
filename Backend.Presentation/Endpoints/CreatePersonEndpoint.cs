using Microsoft.AspNetCore.Builder;
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
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapCreatePersonEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/persons", CreatePersonHandler.HandleAsync)
            .WithName("CreatePerson")
            .WithOpenApi();

        return builder;
    }
}
