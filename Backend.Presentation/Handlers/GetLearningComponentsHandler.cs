using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching learning components for a specific learning space.
    /// </summary>
    public static class GetLearningComponentsHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch learning components for a learning space.
        /// </summary>
        /// <param name="service">Service for accessing learning component data.</param>
        /// <param name="id">The identifier of the learning space (from route).</param>
        /// <returns>An <see cref="IResult"/> response containing the list of components or an error.</returns>
        public static async Task<IResult> HandleAsync(
            [FromServices] ILearningComponentService service,
            [FromRoute] string id)
        {
            try
            {
                // Fetch components from the service
                var components = await service.GetComponentsByLearningSpaceIdAsync(id);

                // Create response containing the list of components, mapped to DTOs
                var response = new GetLearningComponentsResponse(
                    LearningComponentMapper.ToDtoList(components)
                );

                // Returns the response with status 200 OK
                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
            {
                return ErrorResponseHelper.BadRequest("Learning space ID cannot be null or empty");
            }
            catch (KeyNotFoundException ex)
            {
                return ErrorResponseHelper.NotFound(ex.Message);
            }
        }
    }
}
