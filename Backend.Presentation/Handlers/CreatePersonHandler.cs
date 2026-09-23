using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

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
    /// <param name="validator">The DTO validator.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the creation result,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// or a <see cref="Conflict{T}"/> if the person already exists.
    /// </returns>
    public static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> HandleAsync(
        IPersonCreateService service,
        IValidator<CreatePersonDto> validator,
        CreatePersonDto dto)
    {
        return await CreatePersonResponseFactory.ExecuteAsync(service, validator, dto);
    }
}
