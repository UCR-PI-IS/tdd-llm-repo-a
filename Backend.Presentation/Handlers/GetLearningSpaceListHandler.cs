using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers
{
    public static class GetLearningSpaceListHandler
    {
        // Handles GET /learning-spaces/{id}/components
        public static async Task<IResult> HandleGetComponentsAsync([FromServices] ILearningSpaceListService service, [FromRoute] int id)
        {
            try
            {
                var components = service.ListComponents(id);
                var dtos = components.Select(c => new LearningComponentDto { Name = c.Name }).ToList();
                return Results.Ok(dtos);
            }
            catch (InvalidLearningSpaceException ex)
            {
                return Results.NotFound(new ErrorResponse { Message = ex.Message });
            }
        }
    }
}
