using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching learning components for a learning space.
    /// </summary>
    public static class GetLearningSpaceListHandler
    {
        /// <summary>
        /// Handles the asynchronous GET request to retrieve components for a learning space by ID.
        /// </summary>
        /// <param name="learningSpaceId">The ID of the learning space.</param>
        /// <param name="learningSpaceList">Service to retrieve learning space components.</param>
        /// <returns>An HTTP result with either the list of components or an error response.</returns>
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
