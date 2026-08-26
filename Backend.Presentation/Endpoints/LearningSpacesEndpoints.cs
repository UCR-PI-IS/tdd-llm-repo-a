using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning spaces API.
/// </summary>
public static class LearningSpacesEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a new learning space.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningSpacesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/learningspaces",
            async ([FromServices] ILearningSpaceCreateService service, [FromBody] CreateLearningSpaceDto dto) =>
            {
                return await CreateLearningSpaceHandler.HandleAsync(service, dto);
            })
            .WithName("CreateLearningSpace")
            .WithOpenApi()
            .Accepts<CreateLearningSpaceDto>("application/json");

        return builder;
    }
}
