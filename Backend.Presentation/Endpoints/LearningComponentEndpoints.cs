using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

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
    /// <param name="handler">The handler that processes create component requests.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapCreateComponentEndpoint(
        IEndpointRouteBuilder builder,
        Handlers.CreateLearningComponentHandler handler)
    {
        builder.MapPost("/api/components", async (CreateComponentRequest request, HttpContext context) =>
        {
            var requestHandler = context.RequestServices.GetService<Handlers.CreateLearningComponentHandler>() ?? handler;
            var result = await requestHandler.HandleAsync(request);
            return Results.Json(result, statusCode: result.StatusCode);
        })
        .WithName("CreateComponent")
        .WithOpenApi();

        return builder;
    }
}
