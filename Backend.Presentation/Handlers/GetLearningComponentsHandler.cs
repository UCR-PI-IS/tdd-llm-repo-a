using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching learning components for a learning space.
    /// </summary>
    public static class GetLearningComponentsHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch learning components for a specific learning space.
        /// </summary>
        /// <param name="learningComponentService">Service for accessing learning component data.</param>
        /// <param name="learningSpaceId">The identifier of the learning space.</param>
        /// <returns>An <see cref="IResult"/> containing either the list of components or an error response.</returns>
        public static async Task<IResult> HandleAsync(
            [FromServices] ILearningComponentService learningComponentService,
            string learningSpaceId)
        {
            try
            {
                // Fetch all learning components for the specified learning space
                var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

                // Creates a response containing a list of all learning components, mapped to DTOs
                var response = new GetLearningComponentsResponse(
                    components.Select(LearningComponentDto.FromEntity).ToList()
                );

                // Returns the response with status 200 OK
                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex)
            {
                // Returns a bad request response when the learning space ID is invalid
                return TypedResults.BadRequest(new ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                // Returns a not found response when the learning space does not exist
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
