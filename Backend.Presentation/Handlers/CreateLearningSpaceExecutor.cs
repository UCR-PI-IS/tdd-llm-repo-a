using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreateLearningSpaceExecutor
{
    public static async Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> ExecuteAsync(
        ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        try
        {
            var learningSpace = await service.CreateLearningSpaceAsync(
                dto.Type, dto.Height, dto.Width, dto.Length);
            return LearningSpaceResponseMapper.ToCreated(learningSpace);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An unexpected error occurred while creating the learning space.");
        }
    }
}
