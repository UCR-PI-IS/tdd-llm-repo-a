using FluentValidation;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
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
    /// A <see cref="PersonCreationResult"/> wrapping the created person,
    /// a bad request if validation fails,
    /// or a conflict if the person already exists.
    /// </returns>
    public static async Task<PersonCreationResult> HandleAsync(
        IPersonService service,
        CreatePersonDto dto)
    {
        var validationResult = new CreatePersonDtoValidator().Validate(dto);

        if (!validationResult.IsValid)
        {
            return PersonCreationResultFactory.BadRequest(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var person = new Person(
            id: 0,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.IdentityNumber,
            dto.BirthDate);

        var result = await service.CreatePersonAsync(person);

        if (!result.IsSuccess)
        {
            return PersonCreationResultFactory.Conflict(result.ErrorMessage!);
        }

        return PersonCreationResultFactory.Ok(person);
    }
}
