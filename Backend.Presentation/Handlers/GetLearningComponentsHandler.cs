using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

public static class GetLearningComponentsHandler
{
    public static async Task<Results<Ok<GetLearningComponentsResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> HandleAsync(
        ILearningComponentService service, string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            return TypedResults.BadRequest(new ErrorResponse { Message = "Learning space ID cannot be null or empty" });
        }

        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var response = new GetLearningComponentsResponse
            {
                Components = components.ConvertAll(c => new LearningComponentDto
                {
                    ComponentId = c.ComponentId,
                    LearningSpaceId = c.LearningSpaceId,
                    Width = c.Width,
                    Height = c.Height,
                    Depth = c.Depth,
                    X = c.X,
                    Y = c.Y,
                    Z = c.Z,
                    Orientation = c.Orientation
                })
            };
            return TypedResults.Ok(response);
        }
        catch
        {
            return TypedResults.NotFound(new ErrorResponse { Message = learningSpaceId });
        }
    }
}
