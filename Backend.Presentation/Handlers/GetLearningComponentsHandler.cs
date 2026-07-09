using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching a list of learning components.
    /// </summary>
    public static class GetLearningComponentsHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch learning components for a learning space.
        /// </summary>
        /// <param name="service">Service for accessing learning components.</param>
        /// <param name="learningSpaceId">The learning space identifier.</param>
        /// <returns>An <see cref="IResult"/> response containing the list of learning components or an error.</returns>
        public static async Task<IResult> HandleAsync(ILearningComponentService service, string learningSpaceId)
        {
            try
            {
                var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

                var response = new GetLearningComponentsResponse(
                    components.Select(c => new LearningComponentDto(
                        c.ComponentId,
                        c.LearningSpaceId,
                        c.Width,
                        c.Height,
                        c.Depth,
                        c.X,
                        c.Y,
                        c.Z,
                        c.Orientation)).ToList()
                );

                return TypedResults.Ok(response);
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
