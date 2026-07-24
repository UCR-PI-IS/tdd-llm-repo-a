using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Builds BadRequest results for learning component endpoints.
/// </summary>
internal static class LearningComponentBadRequestResult
{
    private const string EmptyIdMessage = "Learning space ID cannot be null or empty.";

    /// <summary>
    /// Creates a BadRequest result for a missing or empty learning space id.
    /// </summary>
    public static IResult Create()
    {
        return TypedResults.BadRequest(new ErrorResponse(EmptyIdMessage));
    }
}
