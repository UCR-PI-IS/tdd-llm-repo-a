using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using System.Net;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for GET /learning-spaces/{learningSpaceId}/components
    /// </summary>
    public static class GetLearningSpaceComponentsHandler
    {
        /// <summary>
        /// Handles request to get components of a learning space.
        /// Returns 200 with component list or 404 with error message on invalid id.
        /// </summary>
        /// <param name="learningSpaceId">The learning space id.</param>
        /// <param name="learningSpaceListService">The application service.</param>
        /// <returns>Ok or NotFound result.</returns>
        public static IResult HandleAsync(int learningSpaceId, [FromServices] ILearningSpaceListService learningSpaceListService)
        {
            try
            {
                var components = learningSpaceListService.ListComponents(learningSpaceId);
                var resultDtos = new System.Collections.Generic.List<LearningComponentDto>();
                foreach(var c in components)
                {
                    resultDtos.Add(new LearningComponentDto(c.Name));
                }
                return TypedResults.Ok(resultDtos);
            }
            catch (System.Exception ex) when (ex is UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions.InvalidLearningSpaceException)
            {
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
