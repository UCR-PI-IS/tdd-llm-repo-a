using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the modification of an existing user with associated person information.
/// </summary>
public static class PutModifyUserWithPersonHandler
{
    /// <summary>
    /// Handles the HTTP PUT request to modify an existing user with person information.
    /// </summary>
    /// <param name="userWithPersonService"> The service for user-person operations.</param>
    /// <param name="request"> The request containing the identity number and user data to be modified.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static async Task<Results<
       Ok<PutModifyUserWithPersonResponse>,
       NotFound<string>,
       BadRequest<ErrorResponse>>> HandleAsync(
       [FromServices] IUserWithPersonService userWithPersonService,
       [FromBody] PutModifyUserWithPersonRequest request)
    {
        var errorMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(request.IdentityNumber))
            errorMessages.Add("Identity number is required.");

        if (request.UserWithPerson == null)
            errorMessages.Add("UserWithPerson data is required.");

        if (errorMessages.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));

        var allUsers = await userWithPersonService.GetAllUserWithPersonAsync();
        var existingUser = allUsers.FirstOrDefault(u => u.IdentityNumber.Value == request.IdentityNumber);

        if (existingUser == null)
            return TypedResults.NotFound($"User with identity number {request.IdentityNumber} not found.");

        UserWithPersonDtoMapper.UpdateEntity(existingUser, request.UserWithPerson);

        var updated = await userWithPersonService.UpdateUserWithPersonAsync(existingUser);

        if (!updated)
            return TypedResults.BadRequest(new ErrorResponse(new List<string> { "Could not update the user." }));

        return TypedResults.Ok(new PutModifyUserWithPersonResponse(request.UserWithPerson));
    }

}