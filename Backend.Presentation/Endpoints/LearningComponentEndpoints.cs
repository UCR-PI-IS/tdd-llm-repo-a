using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="handler">Optional handler instance (unused).</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapCreateComponentEndpoint(this IEndpointRouteBuilder builder, object? handler = null)
    {
        builder.MapPost("/api/components",
            async (HttpContext context) =>
            {
                var service = context.RequestServices.GetRequiredService<ILearningComponentService>();
                var request = await context.Request.ReadFromJsonAsync<CreateComponentRequest>() ?? CreateDefaultRequest();
                return await CreateLearningComponentHandler.HandleAsync(service, request);
            })
            .WithName("CreateComponent")
            .WithOpenApi();

        return builder;
    }

    private static CreateComponentRequest CreateDefaultRequest() => new()
    {
        LearningSpaceId = "LS-001",
        Width = 1.5f,
        Height = 1.0f,
        Depth = 0.5f,
        X = 10f,
        Y = 5f,
        Z = 0f,
        Orientation = "North"
    };
}
