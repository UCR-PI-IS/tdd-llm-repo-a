using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Provides a handler for deleting a user by their Id.
/// </summary>
public static class DeleteUserHandler
{
    /// <summary>
    /// Handles the deletion of a user by their Id.
    /// </summary>
    /// <param name="id">The Id of the user to delete.</param>
    /// <param name="userService">The user service for executing the deletion.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The result contains:
    /// - <see cref="Ok{T}"/> if the user was successfully deleted.
    /// - <see cref="NotFound{T}"/> if no user was found with that Id.
    /// </returns>
    public static async Task<Results<
      Ok<string>,
      NotFound<string>>> HandleAsync(
      [FromRoute] int id,
      [FromServices] IUserService userService)
    {
        var isDeleted = await userService.DeleteUserAsync(id);

        if (!isDeleted)
        {
            return TypedResults.NotFound($"User with Id '{id}' was not found.");
        }

        return TypedResults.Ok("User has been deleted from the system successfully.");
    }
}
