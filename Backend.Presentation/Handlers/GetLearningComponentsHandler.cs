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
        /// <returns>An <see cref="IResult"/> response containing the list of components or an error.</returns>
        public static async Task<IResult> HandleAsync([FromServices] ILearningComponentService service, String learningSpaceId)
        {
            // Validate input
            if (String.IsNullOrEmpty(learningSpaceId))
            {
                return TypedResults.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
            }

            try
            {
                // Fetch components from the service
                var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

                // Map components to DTOs
                var componentDtos = components.Select(c => new LearningComponentDto(c.ComponentId, c.LearningSpaceId)).ToList();

                // Create response
                var response = new GetLearningComponentsResponse(componentDtos);

                // Return OK response
                return TypedResults.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                // Return NotFound if learning space doesn't exist
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
