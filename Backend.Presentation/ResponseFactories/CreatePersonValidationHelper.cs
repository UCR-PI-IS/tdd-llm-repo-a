using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.ResponseFactories;

internal static class CreatePersonValidationHelper
{
    internal static List<string>? GetValidationErrors(CreatePersonDto dto)
    {
        var validator = new CreatePersonDtoValidator();
        var validationResult = validator.Validate(dto);

        if (validationResult.IsValid)
            return null;

        return validationResult.Errors.Select(e => e.ErrorMessage).ToList();
    }
}
