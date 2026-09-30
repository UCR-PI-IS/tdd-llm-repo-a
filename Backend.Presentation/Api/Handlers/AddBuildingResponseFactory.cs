using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class AddBuildingResponseFactory
{
    public static async Task<IResult> CreateResponseAsync(AddBuildingRequest request, IBuildingService service)
    {
        try
        {
            var building = new Building(
                request.Name, request.Color, request.Height, request.Length,
                request.Width, request.X, request.Y, request.Z, request.AreaId);

            var result = await service.AddBuildingAsync(building);
            return TypedResults.Created($"/buildings/{result.InternalId}", new AddBuildingResponse(result));
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest<string>($"{ex.ParamName} cannot be empty");
        }
        catch (DuplicateBuildingException ex)
        {
            return TypedResults.Conflict<string>(ex.Message);
        }
        catch (AreaNotFoundException ex)
        {
            return TypedResults.BadRequest<string>(ex.Message);
        }
    }
}
