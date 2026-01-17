using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    public static class GetLearningSpaceComponentsHandler
    {
        public static async Task<IResult> HandleAsync(
            [FromServices] ILearningSpaceListService service,
            [FromRoute] int id)
        {
            try
            {
                // Minimal sync call, as service layer signature is sync (per prior implementation)
                var components = service.ListComponents(id);
                
                // Map domain objects to DTOs
                var dtoList = components.Select(c => new LearningComponentDto(c.Name)).ToList();
                return Results.Ok(dtoList);
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "InvalidLearningSpaceException")
                {
                    return Results.NotFound(new ErrorResponse { Message = ex.Message });
                }
                throw;
            }
        }
    }

    public record LearningComponentDto(string Name);

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}
