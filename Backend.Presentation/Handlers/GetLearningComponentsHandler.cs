using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching learning components in a learning space.
    /// </summary>
    public static class GetLearningComponentsHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch learning components for a learning space.
        /// </summary>
        /// <param name="learningComponentService">Service for accessing learning components.</param>
        /// <param name="learningSpaceId">The identifier of the learning space.</param>
        /// <returns>A result indicating the outcome of the operation.</returns>
        public static async Task<IResult> HandleAsync([FromServices] ILearningComponentService learningComponentService, string learningSpaceId)
        {
            try
            {
                // Fetch learning components from the service
                var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

                // Map components to DTOs
                var componentDtos = components.Select(c => new LearningComponentDto(
                    c.ComponentId,
                    c.LearningSpaceId,
                    c.Width,
                    c.Height,
                    c.Depth,
                    c.X,
                    c.Y,
                    c.Z,
                    c.Orientation
                )).ToList();

                // Create response
                var response = new GetLearningComponentsResponse(componentDtos);

                // Return OK response
                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
            {
                // Return BadRequest for invalid learning space ID
                return TypedResults.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
            }
            catch (KeyNotFoundException ex)
            {
                // Return NotFound when learning space does not exist
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
