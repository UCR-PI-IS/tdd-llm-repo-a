using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for creating learning components.
/// </summary>
public static class LearningComponentEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a learning component.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <param name="handler">The handler for creating a learning component.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/>.</returns>
    public static IEndpointRouteBuilder MapCreateComponentEndpoint(
        IEndpointRouteBuilder builder,
        ICreateLearningComponentHandler handler)
    {
        builder.MapPost("/api/components", async (CreateComponentRequest request) =>
            {
                var result = await handler.HandleAsync(request);
                return Results.Json(result, statusCode: result.StatusCode);
            })
            .WithName("CreateComponent")
            .WithOpenApi();

        return builder;
    }
}
