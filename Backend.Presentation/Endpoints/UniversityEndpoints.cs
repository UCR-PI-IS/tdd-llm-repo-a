using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for university operations.
/// </summary>
public static class UniversityEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating universities.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapUniversityEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/universities", UniversityEndpointHandler.HandleAsync)
            .WithName("CreateUniversity")
            .WithOpenApi();

        return builder;
    }
}

/// <summary>
/// Request model for creating a university.
/// </summary>
public class CreateUniversityRequest
{
    /// <summary>
    /// Gets or sets the name of the university.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country where the university is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;
}
