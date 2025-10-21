using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement; 
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

/// <summary>
/// Handles the creation of a new student.
/// </summary>
public static class PostCreateStudentHandler
{

    /// <summary>
    /// Handles the HTTP POST request to create a new Student.
    /// </summary>
    /// <param name="StudentService"> The service used to manage Student-related operations.</param>
    /// <param name="request"> The request containing the details of the Student to be created.</param>
    /// <returns></returns>
    public static async Task<Results<Ok<PostCreateStudentResponse>, Conflict, BadRequest<ErrorResponse>>> HandleAsync(
      [FromServices] IStudentService StudentService,
      [FromBody] PostCreateStudentRequest request)
    {
        Student? studentEntity = null;
        List<string> errorMessages = [];
        try
        {
            studentEntity = StudentDtoMapper.ToEntity(request.Student);
        }
        catch (ValidationException exception)
        {
            errorMessages.Add(exception.Message);
        }
        if (errorMessages.Count > 0 || studentEntity == null)
        {
            return TypedResults.BadRequest(
                new ErrorResponse(errorMessages));
        }
        var additionSucceeded = await StudentService.CreateStudentAsync(studentEntity);


        if (!additionSucceeded)
        {
            return TypedResults.Conflict();
        }

        return TypedResults.Ok(new PostCreateStudentResponse(request.Student, "Successful creation."));
    }
}

