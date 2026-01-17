using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    public static class LearningSpaceListEndpoints
    {
        public static IEndpointRouteBuilder MapLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/LearningSpaceList", GetLearningSpaceListHandler.HandleAsync)
                .WithName("GetLearningSpaceList")
                .WithOpenApi();

            // NEW ENDPOINT for component listing as per user story and tests:
            builder.MapGet("/learning-spaces/{id}/components", GetLearningSpaceComponentsHandler.HandleAsync)
                .WithName("GetLearningSpaceComponents")
                .WithOpenApi();
                
            return builder;
        }
    }
}
