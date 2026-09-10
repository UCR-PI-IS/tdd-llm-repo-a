using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning components API.
/// </summary>
public static class LearningComponentEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a learning component.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="handler">The handler for create requests.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapCreateComponentEndpoint(IEndpointRouteBuilder builder, ICreateLearningComponentHandler handler)
    {
        builder.MapPost("/api/components", async (CreateComponentRequest request) => await handler.HandleAsync(request))
            .WithName("CreateComponent");
        return builder;
    }
}
