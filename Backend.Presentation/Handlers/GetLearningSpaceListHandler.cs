using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.IS.ThemePark.Backend.Application.Services;
using UCR.ECCI.IS.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.IS.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.IS.ThemePark.Backend.Presentation.Api.Handlers
{
    /// <summary>
    /// Handler for fetching a list of learning spaces.
    /// </summary>
    public static class GetLearningSpaceListHandler
    {
        
        /*
        public static async Task<Ok<GetLearningSpaceListResponse>> HandleAsync([FromServices] ILearningSpaceListService learningSpaceList)
        {
            // Fetches a single learning space by its id
            var learningSpace = await learningSpaceList.GetCurrentLearningSpaceListAsync();

            // Creates a response with the data of the fetched learning space
            var response = new GetLearningSpaceListResponse(new LearningSpaceDto(learningSpace.id, learningSpace.type));

            // Returns the response with status 200 OK
            return TypedResults.Ok(response);
        }
        */

        /// <summary>
        /// Handles the asynchronous request to fetch all learning spaces.
        /// </summary>
        /// <param name="learningSpaceList">Service for accessing the list of learning spaces.</param>
        /// <returns>An <see cref="Ok{T}"/> response containing the list of all learning spaces.</returns>
        public static async Task<Ok<GetLearningSpaceListResponse>> HandleAsync([FromServices] ILearningSpaceListService learningSpaceList)
        {
            // Fetches all learning spaces from the service
            var spaces = await learningSpaceList.GetAllLearningSpacesAsync();

            // Creates a response containing a list of all learning spaces, mapped to DTOs
            var response = new GetLearningSpaceListResponse(
                spaces.Select(space => new LearningSpaceDto(space.id, space.type)).ToList()
            );

            // Returns the response with status 200 OK
            return TypedResults.Ok(response);
        }
    }
}
