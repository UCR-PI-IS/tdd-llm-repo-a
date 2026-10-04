using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonResponseFactory
{
    public static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> ExecuteAsync(
        IPersonService service,
        CreatePersonDto dto)
    {
        if (!TryValidate(dto, out var errors))
            return BuildBadRequest(errors);

        var person = CreatePersonMapper.ToEntity(dto);
        var result = await service.CreatePersonAsync(person);

        return result.IsSuccess
            ? BuildSuccess()
            : BuildConflict(result.ErrorMessage);
    }

    private static bool TryValidate(CreatePersonDto dto, out IEnumerable<string> errors)
    {
        var validator = new CreatePersonDtoValidator();
        var validationResult = validator.Validate(dto);
        errors = validationResult.Errors.Select(e => e.ErrorMessage);
        return validationResult.IsValid;
    }

    private static Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>> BuildBadRequest(IEnumerable<string> errors)
    {
        return TypedResults.BadRequest(new ValidationErrorResponse(errors));
    }

    private static Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>> BuildSuccess()
    {
        return TypedResults.Ok(new CreatePersonResponse(true));
    }

    private static Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>> BuildConflict(string errorMessage)
    {
        return TypedResults.Conflict(new ErrorResponse(errorMessage));
    }
}
