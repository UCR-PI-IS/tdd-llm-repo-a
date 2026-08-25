using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses related to learning space creation operations.
/// </summary>
internal static class LearningSpaceCreateResponseFactory
{
    /// <summary>
    /// Creates a 201 Created response with the newly created learning space.
    /// </summary>
    /// <param name="space">The created learning space entity.</param>
    /// <returns>A <see cref="IResult"/> representing the 201 Created response.</returns>
    public static IResult CreateCreatedResponse(LearningSpace space)
    {
        var response = new LearningSpaceResponse(
            space.LearningSpaceId,
            space.Type,
            space.Height,
            space.Width,
            space.Length);
        return Results.Created($"/LearningSpaces/{response.LearningSpaceId}", response);
    }

    /// <summary>
    /// Creates a 400 Bad Request response with the validation error message.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    /// <returns>A <see cref="IResult"/> representing the 400 Bad Request response.</returns>
    public static IResult CreateBadRequestResponse(string message)
    {
        return Results.BadRequest(message);
    }

    /// <summary>
    /// Creates a 500 Internal Server Error response for unexpected failures.
    /// </summary>
    /// <returns>A <see cref="IResult"/> representing the 500 Problem response.</returns>
    public static IResult CreateErrorResponse()
    {
        return Results.Problem("An unexpected error occurred while creating the learning space.");
    }
}
