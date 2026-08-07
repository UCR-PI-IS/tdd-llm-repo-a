using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching a list of learning spaces.
    /// </summary>
    public static class GetLearningSpaceListHandler
    {
        /// <summary>
        /// Handles the asynchronous request to fetch all learning spaces.
        /// </summary>
        /// <param name="learningSpaceList">Service for accessing the list of learning spaces.</param>
        /// <returns>An <see cref="Ok{T}"/> response containing the list of all learning spaces.</returns>
        public static async Task<Ok<GetLearningSpaceListResponse>> HandleAsync([FromServices] ILearningSpaceListService learningSpaceList)
        {
            var spaces = await learningSpaceList.GetAllLearningSpacesAsync();
            var spaceDtos = spaces.Select(LearningSpaceDto.FromEntity).ToList();
            return TypedResults.Ok(new GetLearningSpaceListResponse(spaceDtos));
        }
    }
}
