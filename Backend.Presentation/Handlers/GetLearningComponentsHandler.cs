using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching learning components of a learning space.
    /// </summary>
    public static class GetLearningComponentsHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch learning components for a specific learning space.
        /// </summary>
        /// <param name="service">Service for accessing learning components.</param>
        /// <param name="learningSpaceId">The identifier of the learning space.</param>
        /// <returns>An <see cref="Ok{T}"/> response containing the list of components, or an error response.</returns>
        public static async Task<IResult> HandleAsync(
            [FromServices] ILearningComponentService service,
            string learningSpaceId)
        {
            try
            {
                var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
                var componentDtos = components.Select(LearningComponentDto.FromEntity).ToList();
                return TypedResults.Ok(new GetLearningComponentsResponse(componentDtos));
            }
            catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
            {
                return TypedResults.BadRequest(new ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
