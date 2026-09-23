using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.ResponseFactories;

internal static class CreatePersonResponseFactory
{
    internal static async Task<Results<Ok<CreatePersonResponse>, BadRequest<ValidationErrorResponse>, Conflict<ErrorResponse>>> CreateResponseAsync(
        IPersonCreateService service,
        CreatePersonDto dto)
    {
        var validationErrors = CreatePersonValidationHelper.GetValidationErrors(dto);
        if (validationErrors is not null)
        {
            return TypedResults.BadRequest(new ValidationErrorResponse(validationErrors));
        }

        var person = new Person(0, dto.FirstName, dto.LastName, dto.Email, dto.IdentityNumber, dto.BirthDate);
        var result = await service.CreatePersonAsync(person);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(new CreatePersonResponse(true));
        }

        return TypedResults.Conflict(new ErrorResponse(result.ErrorMessage!));
    }
}
