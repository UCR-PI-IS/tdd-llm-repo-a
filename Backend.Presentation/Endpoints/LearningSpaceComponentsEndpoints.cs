using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints;

public static class LearningSpaceComponentsEndpoints
{
    public static IEndpointRouteBuilder MapLearningSpaceComponentsEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/learning-spaces/{id}/components", async (int id, ILearningSpaceListService svc) =>
        {
            try
            {
                var components = svc.ListComponents(id);
                var dtoList = components.ConvertAll(c => new LearningComponentDto(c.Name));
                return Results.Ok(dtoList);
            }
            catch (InvalidLearningSpaceException ex)
            {
                return Results.NotFound(new ErrorResponse(ex.Message));
            }
        });
        return routes;
    }
}
