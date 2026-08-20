using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for learning space creation API.
/// </summary>
public static class CreateLearningSpaceEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a new learning space.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapCreateLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/LearningSpaces",
            (ILearningSpaceCreateService service, CreateLearningSpaceDto dto) =>
            {
                return CreateLearningSpaceHandler.HandleAsync(service, dto);
            })
            .WithName("CreateLearningSpace")
            .WithOpenApi();

        return builder;
    }
}
