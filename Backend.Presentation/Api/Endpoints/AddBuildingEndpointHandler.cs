using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Helper for add-building endpoint logic to keep endpoint coupling low.
/// </summary>
internal static class AddBuildingEndpointHandler
{
    public static async Task<IResult> HandleAsync(IBuildingService service, CreateBuildingDto dto)
    {
        return (IResult)(await AddBuildingHandler.HandleAsync(dto, service)).Result;
    }
}
