using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the deletion of Role associations in the system.
/// </summary>
public static class DeleteRoleHandler
{
    /// <summary>
    /// Handles the HTTP request to delete a role from the system.
    /// </summary>
    /// <param name="request">The request containing the ID of the role to delete.</param>
    /// <param name="RoleService">The service responsible for role management operations.</param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation.  
    /// The result is a <see cref="Results{T1, T2, T3}"/> which can be:
    /// <list type="bullet">
    /// <item><description><see cref="Ok{T}"/> with <see cref="DeleteRoleResponse"/> if the role was deleted successfully.</description></item>
    /// <item><description><see cref="NotFound{T}"/> with <see cref="ErrorResponse"/> if the role with the specified ID does not exist.</description></item>
    /// <item><description><see cref="BadRequest{T}"/> with <see cref="ErrorResponse"/> if the request contains invalid data (e.g., non-positive ID).</description></item>
    /// </list>
    /// </returns>
    public static async Task<Results<
        Ok<DeleteRoleResponse>,
        NotFound<ErrorResponse>,
        BadRequest<ErrorResponse>>> HandleAsync(
        [AsParameters] DeleteRoleRequest request,
        [FromServices] IRoleService RoleService)
    {
        var errorMessages = new List<string>();

        if (request.Id <= 0)
            errorMessages.Add("Role Id must be a positive integer.");

        if (errorMessages.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));

        // Attempt to delete the role association using the service.
        var result = await RoleService.DeleteRoleAsync(request.Id);
        // Return NotFound if the deletion fails.
        if (!result)
            return TypedResults.NotFound(new ErrorResponse(
                new List<string> {
                    $"Role with Id: {request.Id} not found."
                }
            ));
        // Return OK result.
        return TypedResults.Ok(
            new DeleteRoleResponse($"The role with Id: {request.Id} has been deleted successfully.")
            );
    }
}
