using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new person.
/// </summary>
public static class CreatePersonHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new person.
    /// </summary>
    /// <param name="service">The person service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// A <see cref="Ok{T}"/> response with the created person,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="Conflict{T}"/> if the person already exists.
    /// </returns>
    public static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ErrorResponse>, Conflict<ErrorResponse>>> HandleAsync(
        [FromServices] IPersonService service,
        [FromBody] CreatePersonDto dto)
    {
        // Validate the DTO
        var validator = new CreatePersonDtoValidator();
        var validationResult = validator.Validate(dto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var messages = new string[errors.Count];
            for (int i = 0; i < errors.Count; i++)
            {
                messages[i] = errors[i].ErrorMessage;
            }
            var errorMessage = string.Join("; ", messages);
            return TypedResults.BadRequest(new ErrorResponse(errorMessage));
        }

        // Create the person entity
        var personId = $"PER-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        var person = new Person(
            personId,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.IdentityNumber,
            dto.BirthDate);

        // Call the service to create the person
        var result = await service.CreatePersonAsync(person);

        if (!result.IsSuccess)
        {
            return TypedResults.Conflict(new ErrorResponse(result.ErrorMessage ?? "An error occurred while creating the person."));
        }

        return TypedResults.Ok(new CreatePersonResponse(true, personId));
    }
}
