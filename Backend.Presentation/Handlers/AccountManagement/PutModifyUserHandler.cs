using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the HTTP PUT request to modify an existing user in the system.
/// Validates the input, checks for user existence, updates the user entity, and persists the changes.
/// Returns appropriate HTTP responses based on the outcome of the operation.
/// </summary>
public static class PutModifyUserHandler
{
    /// <summary>
    /// Modifies an existing user with the specified ID using the provided user data.
    /// </summary>
    /// <param name="id">The unique identifier of the user to modify.</param>
    /// <param name="userService">The user service used to perform user operations.</param>
    /// <param name="request">The request containing the new user data.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> result with the updated user information if successful,
    /// a <see cref="Conflict"/> result if the modification could not be completed due to a conflict,
    /// or a <see cref="BadRequest{T}"/> result with error details if the request is invalid.
    /// </returns>
    public static async Task<Results<Ok<PutModifyUserResponse>, Conflict, BadRequest<ErrorResponse>>> HandleAsync(
        [FromRoute] int id,
        [FromServices] IUserService userService,
        [FromBody] PutModifyUserRequest request)
    {
        var errorMessages = new List<string>();

        if (id <= 0)
            errorMessages.Add("Invalid user ID.");

        if (request?.User == null || string.IsNullOrWhiteSpace(request.User.UserName) || request.User.UserName == "string")
            errorMessages.Add("Username is required.");

        if (errorMessages.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));

        var users = await userService.GetAllUsersAsync();
        var existingUser = users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
            return TypedResults.BadRequest(new ErrorResponse(new List<string> { "User not found." }));

        UserDtoMapper.UpdateEntity(existingUser, request?.User!);

        var modificationSucceeded = await userService.ModifyUserAsync(id, existingUser);

        if (!modificationSucceeded)
            return TypedResults.Conflict();

        return TypedResults.Ok(new PutModifyUserResponse(request?.User!, "Username successfully modified."));
    }
}
