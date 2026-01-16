using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    public static class GetLearningSpaceListHandler
    {
        public static async Task<Results<Ok<List<LearningComponentDto>>, NotFound<ErrorResponse>>> HandleAsync(
            [FromRoute] int learningSpaceId,
            [FromServices] ILearningSpaceListService learningSpaceList)
        {
            try
            {
                var components = learningSpaceList.ListComponents(learningSpaceId);
                var responseDtos = components.Select(c => new LearningComponentDto(c.Name)).ToList();
                return TypedResults.Ok(responseDtos);
            }
            catch (InvalidLearningSpaceException ex)
            {
                var errorResponse = new ErrorResponse { Message = ex.Message };
                return TypedResults.NotFound(errorResponse);
            }
        }
    }
}
