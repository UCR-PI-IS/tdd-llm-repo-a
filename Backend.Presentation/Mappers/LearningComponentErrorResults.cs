using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Builds error HTTP results for learning component list operations.
/// </summary>
internal static class LearningComponentErrorResults
{
    /// <summary>
    /// Creates a BadRequest result when the learning space id is missing.
    /// </summary>
    public static IResult MissingLearningSpaceId()
    {
        return TypedResults.BadRequest(
            new ErrorResponse("Learning space ID cannot be null or empty."));
    }

    /// <summary>
    /// Creates a NotFound result with the given message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public static IResult NotFound(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }
}
