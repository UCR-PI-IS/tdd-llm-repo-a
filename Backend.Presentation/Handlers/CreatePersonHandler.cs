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
    /// <param name="service">The person creation service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the creation result,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="Conflict{T}"/> if the person already exists.
    /// </returns>
    public static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> HandleAsync(
        IPersonService service,
        CreatePersonDto dto)
    {
        var validator = new CreatePersonDtoValidator();
        var validationResult = validator.Validate(dto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return TypedResults.BadRequest(new ValidationErrorResponse(errors));
        }

        var person = new Person(0, dto.FirstName, dto.LastName, dto.Email, dto.IdentityNumber, dto.BirthDate);
        var result = await service.CreatePersonAsync(person);

        if (!result.IsSuccess)
        {
            return TypedResults.Conflict(new ErrorResponse(result.ErrorMessage));
        }

        return TypedResults.Ok(new CreatePersonResponse(true));
    }
}
