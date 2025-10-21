using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handler for retrieving all registered people.
/// </summary>
public static class GetAllPeopleHandler
{
    /// <summary>
    /// Handles the request to retrieve all people.
    /// </summary>
    /// <param name="personService">The service used to retrieve person data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// An <see cref="Ok{T}"/> result with a <see cref="GetAllPeopleResponse"/>, containing a list (possibly empty) of people.
    /// </returns>
    public static async Task<Results<Ok<GetAllPeopleResponse>, Ok<string>>> HandleAsync(
    [FromServices] IPersonService personService)
    {
        var people = await personService.GetAllPeopleAsync();

        if (people.Count == 0)
        {
            return TypedResults.Ok("There are no registered people.");
        }

        var dtoList = PersonDtoMapper.ToDtoList(people);

        var response = new GetAllPeopleResponse
        {
            People = dtoList
        };

        return TypedResults.Ok(response);
    }
}
