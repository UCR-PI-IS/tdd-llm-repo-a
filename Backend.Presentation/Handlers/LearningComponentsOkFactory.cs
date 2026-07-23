using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

internal static class LearningComponentsOkFactory
{
    public static async Task<IResult> FromServiceAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
        return TypedResults.Ok(LearningComponentDtoMapper.ToResponse(components));
    }
}
