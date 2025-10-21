using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.UniversityManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints.UniversityManagement;

/// <summary>
/// Provides extension methods to map endpoints related to the area module.
/// </summary>
public static class AreaEndpoints
{
    public static IEndpointRouteBuilder MapAreaEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/add-area", PostAreaHandler.HandleAsync)
            .WithName("AddArea")
            .WithTags("Areas")
            .WithOpenApi();

        builder.MapGet("/list-areas", GetAreaHandler.HandleAsync)
            .WithName("GetArea")
            .WithTags("Areas")
            .WithOpenApi();

        builder.MapGet("/list-areas/{areaName}", GetAreaByNameHandler.HandleAsync)
            .WithName("GetAreaByName")
            .WithTags("Areas")
            .WithOpenApi();

        builder.MapDelete("/area/{areaName}", DeleteAreaHandler.HandleAsync)
            .WithName("DeleteArea")
            .WithTags("Areas")
            .WithOpenApi();


        return builder;
    }
}
