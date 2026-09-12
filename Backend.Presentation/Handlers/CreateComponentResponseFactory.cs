using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses related to learning component creation.
/// </summary>
internal static class CreateComponentResponseFactory
{
    /// <summary>
    /// Creates a <see cref="Created{T}"/> response for a successfully created component.
    /// </summary>
    public static IResult Created(CreateComponentResult result) =>
        TypedResults.Created($"/api/components/{result.ComponentId}", new CreateComponentResponse(result.ComponentId, result.Message));

    /// <summary>
    /// Creates an error response from a <see cref="ComponentCreationException"/>.
    /// </summary>
    public static IResult FromException(ComponentCreationException ex) => ex.ErrorType switch
    {
        ComponentCreationErrorType.DuplicateIdError => TypedResults.Conflict(new ErrorResponse(ex)),
        ComponentCreationErrorType.ValidationError => TypedResults.BadRequest(new ErrorResponse(ex)),
        _ => throw ex
    };
}