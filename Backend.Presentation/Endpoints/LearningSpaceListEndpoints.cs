using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints
{
    public static class LearningSpaceListEndpoints
    {
        public static IEndpointRouteBuilder MapLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/learning-spaces/{id}/components", GetLearningSpaceListHandler.HandleGetComponentsAsync)
                .WithName("GetLearningSpaceComponents")
                .WithOpenApi();
            return builder;
        }
    }
}
