using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the creation of a new user.
/// </summary>
public static class PostCreateUserHandler
{

    /// <summary>
    /// Handles the HTTP POST request to create a new user.
    /// </summary>
    /// <param name="userService"> The service used to manage user-related operations.</param>
    /// <param name="request"> The request containing the details of the user to be created.</param>
    /// <returns></returns>
    public static async Task<Results<Ok<PostCreateUserResponse>, Conflict, BadRequest<ErrorResponse>>> HandleAsync(
      [FromServices] IUserService userService,
      [FromBody] PostCreateUserRequest request)
    {
        User? userEntity = null;
        List<string> errorMessages = [];
        try
        {
            userEntity = UserDtoMapper.ToEntity(request.User);
        }
        catch (ValidationException exception)
        {
            errorMessages.Add(exception.Message);
        }
        if (errorMessages.Count > 0 || userEntity == null)
        {
            return TypedResults.BadRequest(
                new ErrorResponse(errorMessages));
        }
        var additionSucceeded = await userService.CreateUserAsync(userEntity);


        if (!additionSucceeded)
        {
            return TypedResults.Conflict();
        }

        return TypedResults.Ok(new PostCreateUserResponse(request.User, "Successful creation."));
    }
}

