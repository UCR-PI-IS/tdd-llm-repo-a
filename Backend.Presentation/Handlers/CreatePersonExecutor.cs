using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonExecutor
{
    internal static async Task<IResult> ExecuteAsync(CreatePersonSetup setup, IPersonService service)
    {
        if (setup.IsValidationError)
        {
            return CreatePersonResponses.ValidationError();
        }

        var result = await service.CreatePersonAsync(setup.Person!);

        if (!result.IsSuccess)
        {
            return CreatePersonResponses.Conflict(result.ErrorMessage);
        }

        return CreatePersonResponses.Success();
    }
}
