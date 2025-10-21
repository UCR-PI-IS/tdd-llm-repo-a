using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the retrieval of user-role associations in the system.
/// </summary>
public static class GetByUserAndRoleHandler
{
    /// <summary>
    /// Handles the HTTP GET request to retrieve a user-role association by user ID and role ID.
    /// </summary>
    /// <returns>
    /// A result containing the user-role association or a NotFound result with an error message.
    /// </returns>
    public static async Task<Results<
        Ok<GetByUserAndRoleResponse>,
        NotFound<ErrorResponse>,
        BadRequest<ErrorResponse>>> HandleAsync(
        [FromRoute] int userId,
        [FromRoute] int roleId,
        [FromServices] IUserRoleService userRoleService)
    {

        var errorMessages = new List<string>();
        if (userId <= 0)
        {
            errorMessages.Add("UsedId must be a positive integer");

        }
        if (roleId <= 0)
        {
            errorMessages.Add("RoleId must be a positive integer");

        }
        if (errorMessages.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));

        // Attempt to retrieve the user-role association using the service.
        var result = await userRoleService.GetByUserAndRoleAsync(userId, roleId);
        // Return NotFound if the retrieval fails.
        if (result is null)
            return TypedResults.NotFound(new ErrorResponse(
                new List<string> {
                    $"No user-role association found for User ID {userId} and Role ID {roleId}."
                }
            ));

        var dto = UserRoleDtoMapper.ToDto(result);
        // Return OK result with the retrieved user-role association.
        return TypedResults.Ok(new GetByUserAndRoleResponse($"User-role association for User ID {userId} and Role ID {roleId} found."));

    }
}
