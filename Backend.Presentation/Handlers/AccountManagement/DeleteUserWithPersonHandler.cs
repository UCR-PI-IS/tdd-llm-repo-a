using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

public static class DeleteUserWithPersonHandler
{
    /// <summary>
    /// Handles the HTTP DELETE request to remove a user with person by their IDs.
    /// </summary>
    /// <param name="userWithPersonService">The service used to manage user-person-related operations.</param>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>
    /// Returns 200 OK with a success message if the user was deleted,  
    /// or 404 Not Found with an error message if the user was not found or could not be deleted.
    /// </returns>
    public static async Task<Results<Ok<string>, NotFound<string>>> HandleAsync(
        [AsParameters] DeleteUserWithPersonRequest request,
        [FromServices] IUserWithPersonService userWithPersonService)
    {

        var deleted = await userWithPersonService.DeleteUserWithPersonAsync(request.UserId,request.PersonId);

        if (!deleted)
        {
            return TypedResults.NotFound($"User with ID {request.UserId} was not found or could not be deleted.");
        }

         return TypedResults.Ok($"User with ID {request.UserId} was successfully deleted.");
     
    }
}
