using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Class-based route handler builder that supports implicit conversion to/from
/// <see cref="RouteHandlerBuilder"/> so mock setups in tests compile correctly.
/// </summary>
public class IRouteHandlerBuilder
{
    private readonly RouteHandlerBuilder _builder;

    // Parameterless constructor for Moq proxy generation
    protected IRouteHandlerBuilder()
    {
        _builder = null!;
    }

    private IRouteHandlerBuilder(RouteHandlerBuilder builder)
    {
        _builder = builder;
    }

    public static implicit operator IRouteHandlerBuilder(RouteHandlerBuilder builder)
        => new IRouteHandlerBuilder(builder);

    public static implicit operator RouteHandlerBuilder(IRouteHandlerBuilder handler)
        => handler._builder;

    /// <summary>
    /// Sets the name of the endpoint.
    /// </summary>
    /// <param name="name">The endpoint name.</param>
    /// <returns>The route handler builder.</returns>
    public virtual IRouteHandlerBuilder WithName(string name)
    {
        _builder.WithName(name);
        return this;
    }

    /// <summary>
    /// Adds OpenAPI metadata to the endpoint.
    /// </summary>
    /// <returns>The route handler builder.</returns>
    public virtual IRouteHandlerBuilder WithOpenApi()
    {
        _builder.WithOpenApi();
        return this;
    }
}

/// <summary>
/// Contains endpoint mappings for the learning components API.
/// </summary>
public static class LearningComponentEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating learning components.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="handler">The handler for creating components.</param>
    /// <returns>The updated route handler builder.</returns>
    public static IRouteHandlerBuilder MapCreateComponentEndpoint(
        IEndpointRouteBuilder builder,
        dynamic handler)
    {
        var route = builder.MapPost("/api/components", async (Dtos.CreateComponentRequest request) => await handler.HandleAsync(request));
        route.WithName("CreateComponent");
        route.WithOpenApi();
        return route;
    }
}
