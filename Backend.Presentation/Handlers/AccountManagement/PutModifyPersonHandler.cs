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
/// Handles the modification of an existing person.
/// </summary>
public static class PutModifyPersonHandler
{
    /// <summary>
    /// Handles the HTTP PUT request to modify an existing person identified by their identity number.
    /// </summary>
    /// <param name="personService">The service used to interact with person entities.</param>
    /// <param name="identityNumber">The identity number of the person to be modified.</param>
    /// <param name="request">The request containing the updated person information.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// <list type="bullet">
    /// <item><see cref="Ok{T}"/> with a <see cref="PutModifyPersonResponse"/> if the person was successfully modified.</item>
    /// <item><see cref="NotFound{T}"/> if the person with the given identity number does not exist.</item>
    /// <item><see cref="BadRequest{T}"/> with an <see cref="ErrorResponse"/> if the request is invalid or no fields were modified.</item>
    /// </list>
    /// </returns>
    public static async Task<Results<Ok<PutModifyPersonResponse>, NotFound<string>, BadRequest<ErrorResponse>>> HandleAsync(
        [FromServices] IPersonService personService,
        [FromRoute] string identityNumber,
        [FromBody] PutModifyPersonRequest request)
    {
        var errorMessages = new List<string>();

        IdentityNumber? validatedIdentityNumber = null;

        try
        {
            validatedIdentityNumber = IdentityNumber.Create(identityNumber);
        }
        catch (ValidationException ex)
        {
            errorMessages.Add(ex.Message);
        }

        if (errorMessages.Count > 0 || validatedIdentityNumber == null)
        {
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));
        }

        var existingPerson = await personService.GetPersonByIdAsync(validatedIdentityNumber.Value);
        if (existingPerson == null)
        {
            return TypedResults.NotFound($"Person with IdentityNumber {validatedIdentityNumber.Value} not found.");
        }

        var dto = new PersonDto(
            existingPerson.Id,
            request.Person.Email != "string" ? request.Person.Email : existingPerson.Email.Value,
            request.Person.FirstName != "string" ? request.Person.FirstName : existingPerson.FirstName,
            request.Person.LastName != "string" ? request.Person.LastName : existingPerson.LastName,
            request.Person.Phone != "string" ? request.Person.Phone : existingPerson.Phone.Value,
            request.Person.BirthDate != DateOnly.FromDateTime(DateTime.Now) ? request.Person.BirthDate : existingPerson.BirthDate.Value,
            request.Person.IdentityNumber != "string" ? request.Person.IdentityNumber : existingPerson.IdentityNumber.Value
        );

        bool wasModified =
            request.Person.Email != existingPerson.Email.Value ||
            request.Person.FirstName != existingPerson.FirstName ||
            request.Person.LastName != existingPerson.LastName ||
            request.Person.Phone != existingPerson.Phone.Value ||
            request.Person.BirthDate != existingPerson.BirthDate.Value ||
            request.Person.IdentityNumber != existingPerson.IdentityNumber.Value;


        if (!wasModified)
        {
            errorMessages.Add("At least one field must be modified.");
            return TypedResults.BadRequest(new ErrorResponse(errorMessages));
        }

        Person updatedPerson;
        try
        {
            updatedPerson = PersonDtoMapper.ToEntity(dto);
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse([ex.Message]));
        }


        var modificationSucceeded = await personService.ModifyPersonAsync(validatedIdentityNumber.Value, updatedPerson);
        if (!modificationSucceeded)
        {
            return TypedResults.NotFound($"Person with IdentityNumber {validatedIdentityNumber.Value} not found.");
        }

        return TypedResults.Ok(new PutModifyPersonResponse(PersonDtoMapper.ToDto(updatedPerson)));
    }
}