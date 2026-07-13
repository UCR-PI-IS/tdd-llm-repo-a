using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching learning components of a learning space.
    /// </summary>
    public static class GetLearningComponentsHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch learning components for a learning space.
        /// </summary>
        /// <param name="service">Service for accessing learning components.</param>
        /// <param name="learningSpaceId">The identifier of the learning space.</param>
        /// <returns>An <see cref="IResult"/> containing either the components list, bad request, or not found response.</returns>
        public static async Task<IResult> HandleAsync(
            [FromServices] ILearningComponentService service,
            string learningSpaceId)
        {
            // Validate input
            if (string.IsNullOrEmpty(learningSpaceId))
            {
                return TypedResults.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
            }

            try
            {
                // Fetch components from service and map to DTOs
                var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
                var componentDtos = LearningComponentMapper.ToDtoList(components);

                // Create response
                var response = new GetLearningComponentsResponse(componentDtos);

                return TypedResults.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
