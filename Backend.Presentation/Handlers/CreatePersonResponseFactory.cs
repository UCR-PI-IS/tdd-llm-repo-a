using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses related to person creation operations.
/// </summary>
internal static class CreatePersonResponseFactory
{
    /// <summary>
    /// Creates a bad request result from validation failures.
    /// </summary>
    /// <param name="validationResult">The validation result containing errors.</param>
    /// <returns>A handler result wrapping a bad request response.</returns>
    public static CreatePersonHandlerResult CreateValidationErrorResponse(ValidationResult validationResult)
    {
        var errorResponse = new ValidationErrorResponse
        {
            Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
        };
        return new CreatePersonHandlerResult(TypedResults.BadRequest(errorResponse));
    }

    /// <summary>
    /// Creates a success result for a successfully created person.
    /// </summary>
    /// <returns>A handler result wrapping an OK response.</returns>
    public static CreatePersonHandlerResult CreateSuccessResponse()
    {
        return new CreatePersonHandlerResult(TypedResults.Ok(new CreatePersonResponse { Success = true }));
    }

    /// <summary>
    /// Creates a conflict result for a duplicate person.
    /// </summary>
    /// <param name="errorMessage">The error message describing the conflict.</param>
    /// <returns>A handler result wrapping a conflict response.</returns>
    public static CreatePersonHandlerResult CreateConflictResponse(string errorMessage)
    {
        return new CreatePersonHandlerResult(TypedResults.Conflict(new ErrorResponse(errorMessage)));
    }
}
