using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning component.
/// </summary>
public class CreateLearningComponentHandler
{
    private readonly ILearningComponentService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLearningComponentHandler"/> class.
    /// </summary>
    /// <param name="service">The learning component service dependency.</param>
    public CreateLearningComponentHandler(ILearningComponentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Handles the asynchronous request to create a new learning component.
    /// </summary>
    /// <param name="request">The create component request.</param>
    /// <returns>A response containing the created component details or an error.</returns>
    public async Task<CreateLearningComponentResponse> HandleAsync(CreateComponentRequest request)
    {
        try
        {
            var component = await _service.CreateComponentAsync(request);

            var message = string.IsNullOrEmpty(request.ComponentId)
                ? $"Component created with auto-generated ID '{component.ComponentId}'"
                : $"Component created with ID '{component.ComponentId}'";

            return new CreateLearningComponentResponse
            {
                ComponentId = component.ComponentId,
                Message = message,
                StatusCode = 201
            };
        }
        catch (DuplicateIdException ex)
        {
            return new CreateLearningComponentResponse
            {
                StatusCode = 409,
                ErrorMessage = ex.Message
            };
        }
        catch (ValidationException ex)
        {
            return new CreateLearningComponentResponse
            {
                StatusCode = 400,
                ErrorMessage = ex.Message
            };
        }
    }
}
