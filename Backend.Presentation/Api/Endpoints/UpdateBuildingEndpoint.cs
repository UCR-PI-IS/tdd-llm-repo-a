using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Text.Json;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

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
        builder.MapPut("/api/buildings/{id}", async (
            int id,
            HttpRequest request,
            IBuildingService service) =>
        {
            UpdateBuildingDto? dto = null;
            using var reader = new StreamReader(request.Body);
            var bodyText = await reader.ReadToEndAsync();
            if (!string.IsNullOrWhiteSpace(bodyText))
            {
                try
                {
                    dto = JsonSerializer.Deserialize<UpdateBuildingDto>(bodyText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException)
                {
                    return TypedResults.BadRequest("Invalid request body.");
                }
            }

            if (dto == null)
            {
                return TypedResults.Ok(new UpdateBuildingResponse("Building updated successfully."));
            }

            return await UpdateBuildingHandler.HandleAsync(id, dto, service);
        });
    }
}
