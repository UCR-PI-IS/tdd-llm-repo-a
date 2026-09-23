using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonResponseFactory
{
    public static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> ExecuteAsync(
        IPersonCreateService service,
        IValidator<CreatePersonDto> validator,
        CreatePersonDto dto)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return TypedResults.BadRequest(new ValidationErrorResponse(errors));
        }

        var person = CreatePersonMapper.ToDomain(dto);
        var result = await service.CreatePersonAsync(person);

        if (!result.IsSuccess)
        {
            return TypedResults.Conflict(new ErrorResponse(result.ErrorMessage!));
        }

        return TypedResults.Ok(new CreatePersonResponse(true));
    }
}
