using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the deletion of Role Permisson associations in the system.
/// </summary>
public static class DeleteRolePermissionHandler
{
    /// <summary>
    /// Processes a request to remove a permission from a role.
    /// </summary>
    /// <param name="request">Contains the role and permission IDs.</param>
    /// <param name="rolePermissionService">Service to manage role-permission operations.</param>
    /// <returns>
    /// A result indicating success (<see cref="Ok{T}"/>),  
    /// not found (<see cref="NotFound{T}"/>),  
    /// or invalid input (<see cref="BadRequest{T}"/>).
    /// </returns>
    public static async Task<Results<
        Ok<DeleteRolePermissionResponse>,
        NotFound<ErrorResponse>,
        BadRequest<ErrorResponse>>> HandleAsync(
        [AsParameters] DeleteRolePermissionRequest request,
        [FromServices] IRolePermissionService rolePermissionService)
    {
        var errorMessages = new List<string>();

        if (request.PermId <= 0)
            errorMessages.Add("PermId must be a positive number.");

        if (request.RoleId <= 0)
            errorMessages.Add("RoleId must be a positive number.");

        if (errorMessages.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));

        // Attempt to delete the role-permission association using the service.
        var result = await rolePermissionService.RemovePermissionFromRoleAsync(request.RoleId, request.PermId);
        // Return NotFound if the deletion fails.
        if (!result)
            return TypedResults.NotFound(new ErrorResponse(
                new List<string> {
                    $"Role ID {request.RoleId} does not have Permission ID {request.PermId} assigned."
                }
            ));
        // Return OK result.
        return TypedResults.Ok(
            new DeleteRolePermissionResponse($"The role-permission association for RoleId {request.RoleId} and PermId {request.PermId} has been deleted successfully.")
            );
    }
}
