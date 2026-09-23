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
public class CreatePersonHandler
{
    private readonly IPersonService _service;
    private readonly CreatePersonDtoValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonHandler"/> class.
    /// </summary>
    /// <param name="service">The person service.</param>
    /// <param name="validator">The DTO validator.</param>
    public CreatePersonHandler(IPersonService service, CreatePersonDtoValidator validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>
    /// Handles the asynchronous request to create a new person.
    /// </summary>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the success response,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="Conflict{T}"/> if the person already exists.
    /// </returns>
    public async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> HandleAsync(CreatePersonDto dto)
    {
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(new ValidationErrorResponse(validationResult.Errors));
        }

        var person = new Person(0, dto.FirstName, dto.LastName, dto.Email, dto.IdentityNumber, dto.BirthDate);
        var result = await _service.CreatePersonAsync(person);

        if (!result.IsSuccess)
        {
            return TypedResults.Conflict(new ErrorResponse(result.ErrorMessage!));
        }

        return TypedResults.Ok(new CreatePersonResponse(true));
    }
}
