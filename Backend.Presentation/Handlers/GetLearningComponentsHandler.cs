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
        /// Handles the asynchronous request to fetch learning components for a specific learning space.
        /// </summary>
        /// <param name="service">Service for accessing learning components.</param>
        /// <param name="id">The identifier of the learning space.</param>
        /// <returns>An <see cref="IResult"/> response containing the list of learning components or an error.</returns>
        public static async Task<IResult> HandleAsync(
            [FromServices] ILearningComponentService service,
            string id)
        {
            try
            {
                var components = await service.GetComponentsByLearningSpaceIdAsync(id);
                var response = LearningComponentMapper.MapToResponse(components);
                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
            {
                return TypedResults.BadRequest(new ErrorResponse($"Learning space ID cannot be null or empty: {id}"));
            }
            catch (KeyNotFoundException ex)
            {
                return TypedResults.NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
