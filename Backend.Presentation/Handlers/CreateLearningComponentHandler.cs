using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Result of a create learning component operation.
/// </summary>
/// <param name="ComponentId">The component identifier (null on error).</param>
/// <param name="StatusCode">The HTTP status code.</param>
/// <param name="ErrorMessage">The error message (null on success).</param>
public record CreateLearningComponentResult(
    string? ComponentId,
    int StatusCode,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Converts this result to an HTTP response result.
    /// </summary>
    /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
    public IResult ToHttpResult()
    {
        return StatusCode switch
        {
            200 => Results.Ok(this),
            409 => Results.Conflict(ErrorMessage),
            400 => Results.BadRequest(ErrorMessage),
            _ => Results.StatusCode(StatusCode)
        };
    }
}

/// <summary>
/// Handler for creating a new learning component.
/// </summary>
public class CreateLearningComponentHandler
{
    private readonly ILearningComponentService _learningComponentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLearningComponentHandler"/> class.
    /// </summary>
    /// <param name="learningComponentService">The learning component service dependency.</param>
    public CreateLearningComponentHandler(ILearningComponentService learningComponentService)
    {
        _learningComponentService = learningComponentService;
    }

    /// <summary>
    /// Handles the asynchronous request to create a new learning component.
    /// </summary>
    /// <param name="request">The creation request containing component parameters.</param>
    /// <returns>A result containing the component ID on success, or an error status code on failure.</returns>
    public async Task<CreateLearningComponentResult> HandleAsync(CreateComponentRequest request)
    {
        try
        {
            var component = await _learningComponentService.CreateComponentAsync(request);
            return new CreateLearningComponentResult(component.ComponentId, 200);
        }
        catch (DuplicateIdException ex)
        {
            return new CreateLearningComponentResult(null, 409, ex.Message);
        }
        catch (ValidationException ex)
        {
            return new CreateLearningComponentResult(null, 400, ex.Message);
        }
    }
}
