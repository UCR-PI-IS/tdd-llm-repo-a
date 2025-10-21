using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.UniversityManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints.UniversityManagement;

/// <summary>
/// Provides extension methods to map endpoints related to the building module.
/// </summary>
public static class BuildingEndpoints
{
    public static IEndpointRouteBuilder MapBuildingEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/add-building", PostBuildingHandler.HandleAsync)
            .WithName("AddBuilding")
            .WithTags("Buildings")
            .WithOpenApi();

        builder.MapGet("/list-buildings", GetBuildingHandler.HandleAsync)
            .WithName("GetBuildingsInformation")
            .WithTags("Buildings")
            .WithOpenApi();

        builder.MapPut("/update-building/{buildingId}", PutBuildingHandler.HandleAsync)
            .WithName("PutBuilding")
            .WithTags("Buildings")
            .WithOpenApi();

        builder.MapGet("/list-building/{buildingId}", GetBuildingByIdHandler.HandleAsync)
            .WithName("GetBuildingById")
            .WithTags("Buildings")
            .WithOpenApi();

        builder.MapDelete("/delete-building/{buildingId}", DeleteBuildingHandler.HandleAsync)
            .WithName("DeleteBuilding")
            .WithTags("Buildings")
            .WithOpenApi();

        return builder;
    }
}
