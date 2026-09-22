using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonHandlerLogic
{
    public static async Task<CreatePersonHandlerResult> ExecuteAsync(IPersonService service, CreatePersonDto dto)
    {
        var setup = CreatePersonSetupFactory.Prepare(dto);
        var result = await CreatePersonExecutor.ExecuteAsync(setup, service);
        return new CreatePersonHandlerResult(result);
    }
}
