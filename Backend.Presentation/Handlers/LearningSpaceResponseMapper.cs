using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class LearningSpaceResponseMapper
{
    public static Created<LearningSpaceResponse> ToCreated(LearningSpace learningSpace)
    {
        var response = new LearningSpaceResponse(
            learningSpace.LearningSpaceId,
            learningSpace.Type,
            learningSpace.Height,
            learningSpace.Width,
            learningSpace.Length);
        return TypedResults.Created($"/LearningSpaces/{response.LearningSpaceId}", response);
    }
}
