using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> HandleAsync(
        IPersonService service,
        CreatePersonDto dto)
    {
        // Validate the DTO
        var validator = new CreatePersonDtoValidator();
        var validationResult = validator.Validate(dto);

        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(new ValidationErrorResponse
            {
                Errors = validationResult.Errors.ToList()
            });
        }

        // Create the person entity
        var person = new Person(
            Guid.NewGuid(),
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.IdentityNumber,
            dto.BirthDate);

        // Call the service to create the person
        var result = await service.CreatePersonAsync(person);

        if (!result.IsSuccess)
        {
            return TypedResults.Conflict(new ErrorResponse(result.ErrorMessage ?? "An error occurred."));
        }

        return TypedResults.Ok(new CreatePersonResponse
        {
            Success = true,
            PersonId = result.PersonId
        });
    }
}
