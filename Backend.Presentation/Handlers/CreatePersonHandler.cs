using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Handler for creating a new person.
/// </summary>
public class CreatePersonHandler
{
    private readonly IPersonService _personService;

    /// <summary>
    /// Initializes a new instance of the CreatePersonHandler class.
    /// </summary>
    /// <param name="personService">The person service.</param>
    public CreatePersonHandler(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Handles the create person request.
    /// </summary>
    /// <param name="request">The create person request.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    public async Task<CreatePersonResponse> HandleAsync(CreatePersonRequest request)
    {
        try
        {
            var result = await _personService.CreatePersonAsync(
                request.FirstName!,
                request.LastName!,
                request.Email!,
                request.IdentityNumber!,
                request.BirthDate,
                request.Phone
            );

            return new CreatePersonResponse
            {
                StatusCode = 200,
                Success = true,
                Message = "Person created successfully"
            };
        }
        catch (InvalidOperationException ex)
        {
            return new CreatePersonResponse
            {
                StatusCode = 400,
                Success = false,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new CreatePersonResponse
            {
                StatusCode = 400,
                Success = false,
                Message = ex.Message
            };
        }
    }
}
