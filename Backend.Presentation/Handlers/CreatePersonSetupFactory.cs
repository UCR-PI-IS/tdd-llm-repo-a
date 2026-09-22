using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonSetupFactory
{
    public static CreatePersonSetup Prepare(CreatePersonDto dto)
    {
        if (!CreatePersonValidatorHelper.IsValid(dto))
        {
            return new CreatePersonSetup(true, null);
        }

        return new CreatePersonSetup(false, CreatePersonEntityFactory.Create(dto));
    }
}
