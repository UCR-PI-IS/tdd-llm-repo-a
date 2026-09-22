using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonValidatorHelper
{
    public static bool IsValid(CreatePersonDto dto)
    {
        return new CreatePersonDtoValidator().Validate(dto).IsValid;
    }
}
