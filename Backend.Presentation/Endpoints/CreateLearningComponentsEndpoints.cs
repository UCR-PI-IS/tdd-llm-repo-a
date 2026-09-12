using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for creating learning components.
/// </summary>
public static class CreateLearningComponentsEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating learning components.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapCreateLearningComponentsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/components", CreateLearningComponentHandler.HandleAsync)
            .WithName("CreateComponent")
            .WithOpenApi();

        return builder;
    }
}
