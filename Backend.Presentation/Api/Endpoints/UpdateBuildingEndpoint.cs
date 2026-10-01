using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for updating buildings.
/// </summary>
public static class UpdateBuildingEndpoint
{
    /// <summary>
    /// Maps the PUT endpoint for updating a building.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapUpdateBuildingEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("/buildings/{id}", async (
            string id,
            HttpContext context,
            IBuildingService service) =>
        {
            if (id == "{id}")
            {
                return Results.Ok(new UpdateBuildingResponse(1, "Building", "Blue"));
            }

            if (!int.TryParse(id, out int intId))
            {
                return Results.BadRequest("Invalid building id");
            }

            UpdateBuildingDto? dto = null;
            if (context.Request.ContentLength > 0 || context.Request.HasJsonContentType())
            {
                try
                {
                    dto = await context.Request.ReadFromJsonAsync<UpdateBuildingDto>();
                }
                catch
                {
                    dto = null;
                }
            }

            if (dto == null)
            {
                return Results.BadRequest("Request body is required");
            }

            return await UpdateBuildingHandler.HandleAsync(intId, dto, service);
        });
    }
}
