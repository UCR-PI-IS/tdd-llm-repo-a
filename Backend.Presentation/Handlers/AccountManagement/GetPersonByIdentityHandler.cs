using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Provides a handler for retrieving a person by their identity number.
/// </summary>
public static class GetPersonByIdentityHandler
{
    /// <summary>
    /// Handles the request to retrieve a person by their identity number.
    /// </summary>
    /// <param name="identityNumber">The unique identity number of the person.</param>
    /// <param name="personService">The service used to retrieve person information.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// <list type="bullet">
    /// <item><see cref="Ok{T}"/> with a <see cref="GetPersonByIdentityResponse"/> if the person is found.</item>
    /// <item><see cref="NotFound{T}"/> if the person is not found.</item>
    /// <item><see cref="BadRequest{T}"/> with an <see cref="ErrorResponse"/> if the identity number is invalid.</item>
    /// </list>
    /// </returns>
    public static async Task<Results<
      Ok<GetPersonByIdentityResponse>,
      NotFound<string>,
      BadRequest<ErrorResponse>>> HandleAsync(
      [FromRoute] string identityNumber,
      [FromServices] IPersonService personService)
    {
        var errorMessages = new List<string>();
        IdentityNumber? identityObj = null;

        try
        {
            identityObj = IdentityNumber.Create(identityNumber);
        }
        catch (ValidationException ex)
        {
            errorMessages.Add(ex.Message);
        }

        if (errorMessages.Count > 0)
        {
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));
        }

        var person = await personService.GetPersonByIdAsync(identityObj!.Value);

        if (person is null)
        {
            return TypedResults.NotFound($"Person with identity number '{identityNumber}' was not found.");
        }

        var dto = PersonDtoMapper.ToDto(person);

        return TypedResults.Ok(new GetPersonByIdentityResponse(dto));
    }
}