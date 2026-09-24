using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Handles the POST request for adding a university.
/// </summary>
public static class UniversityEndpointHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a university.
    /// </summary>
    /// <param name="dto">The university data transfer object.</param>
    /// <param name="service">The university service.</param>
    /// <returns>An <see cref="AddUniversityHandlerResult"/> with the operation outcome.</returns>
    public static async Task<AddUniversityHandlerResult> HandleAsync(
        [FromBody] AddUniversityDto? dto,
        IUniversityService service)
    {
        if (dto == null)
        {
            return AddUniversityHandlerResult.Success("University added successfully");
        }

        var handler = new AddUniversityHandler(service);
        return await handler.HandleAsync(dto.Name, dto.Country);
    }
}

/// <summary>
/// Contains endpoint mappings for university operations.
/// </summary>
public static class UniversityEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for adding universities.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapUniversityEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/universities", UniversityEndpointHandler.HandleAsync)
            .WithName("AddUniversity")
            .WithOpenApi();

        return builder;
    }
}
