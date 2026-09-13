using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

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
/// Contains endpoint mappings for the learning component creation API.
/// </summary>
public static class LearningComponentEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating learning components.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The route handler builder for chaining.</returns>
    public static IRouteHandlerBuilder MapCreateComponentEndpoint(IEndpointRouteBuilder builder)
    {
        return builder.MapPost("/api/components", async (ILearningComponentService service, CreateLearningComponentDto dto) =>
        {
            var result = await CreateLearningComponentHandler.HandleAsync(service, dto);
            if (result.StatusCode == 409)
            {
                return Results.Conflict(new ErrorResponse(result.ErrorMessage!));
            }
            if (result.StatusCode == 400)
            {
                return Results.BadRequest(new ErrorResponse(result.ErrorMessage!));
            }

            return Results.Created($"/api/components/{result.ComponentId}", result);
        })
        .WithName("CreateComponent")
        .WithOpenApi();
    }
}
