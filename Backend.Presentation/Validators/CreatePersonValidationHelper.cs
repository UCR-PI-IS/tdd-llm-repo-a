using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

internal static class CreatePersonValidationHelper
{
    public static IEnumerable<string>? ValidateAndGetErrors(CreatePersonDto dto)
    {
        var validator = new CreatePersonDtoValidator();
        var result = validator.Validate(dto);

        if (result.IsValid)
            return null;

        return result.Errors.Select(e => e.ErrorMessage);
    }
}
