using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

internal static class LearningComponentsErrorFactory
{
    public static IResult EmptyLearningSpaceId() =>
        TypedResults.BadRequest(new ErrorResponse
        {
            Message = "Learning space ID cannot be null or empty"
        });

    public static IResult FromMessage(string message) =>
        TypedResults.BadRequest(new ErrorResponse { Message = message });

    public static IResult LearningSpaceNotFound(string learningSpaceId) =>
        TypedResults.NotFound(new ErrorResponse
        {
            Message = $"Learning space {learningSpaceId} not found"
        });
}
