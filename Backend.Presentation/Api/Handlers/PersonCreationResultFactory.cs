using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating <see cref="PersonCreationResult"/> instances.
/// Centralizes response construction to reduce class coupling in the handler.
/// </summary>
public static class PersonCreationResultFactory
{
    /// <summary>
    /// Creates a BadRequest result for validation failures.
    /// </summary>
    public static PersonCreationResult BadRequest(IEnumerable<string> errors)
    {
        return new PersonCreationResult(
            TypedResults.BadRequest(new ValidationErrorResponse("Validation failed", errors.ToList())));
    }

    /// <summary>
    /// Creates a Conflict result with the specified error message.
    /// </summary>
    public static PersonCreationResult Conflict(string message)
    {
        return new PersonCreationResult(
            TypedResults.Conflict(new ErrorResponse(message)));
    }

    /// <summary>
    /// Creates an Ok result from the created person.
    /// </summary>
    public static PersonCreationResult Ok(Person person)
    {
        return new PersonCreationResult(
            TypedResults.Ok(new CreatePersonResponse(
                true,
                person.Id,
                person.FirstName,
                person.LastName,
                person.Email)));
    }
}
