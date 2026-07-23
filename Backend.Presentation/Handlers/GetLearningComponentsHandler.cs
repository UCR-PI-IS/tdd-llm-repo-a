using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

public static class GetLearningComponentsHandler
{
    public static async Task<IResult> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        if (!LearningSpaceIdValidator.IsValid(learningSpaceId))
            return LearningComponentsErrorFactory.EmptyLearningSpaceId();

        try
        {
            return await LearningComponentsOkFactory.FromServiceAsync(service, learningSpaceId);
        }
        catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
        {
            return LearningComponentsErrorFactory.FromMessage(ex.Message);
        }
        catch
        {
            return LearningComponentsErrorFactory.LearningSpaceNotFound(learningSpaceId);
        }
    }
}
