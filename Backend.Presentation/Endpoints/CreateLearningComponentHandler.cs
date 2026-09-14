using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Handles requests to create a new learning component.
/// </summary>
public class CreateLearningComponentHandler
{
    private readonly ILearningComponentService _learningComponentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLearningComponentHandler"/> class.
    /// </summary>
    /// <param name="learningComponentService">The learning component service.</param>
    public CreateLearningComponentHandler(ILearningComponentService learningComponentService)
    {
        _learningComponentService = learningComponentService;
    }

    /// <summary>
    /// Handles the asynchronous request to create a new learning component.
    /// </summary>
    /// <param name="request">The request containing component data.</param>
    /// <returns>A response containing the created component ID or error information.</returns>
    public async Task<CreateLearningComponentResponse> HandleAsync(CreateComponentRequest request)
    {
        try
        {
            var component = await _learningComponentService.CreateComponentAsync(request);
            
            var message = string.IsNullOrEmpty(request.ComponentId)
                ? $"Component created successfully with auto-generated ID: {component.ComponentId}"
                : $"Component created successfully with ID: {component.ComponentId}";

            return new CreateLearningComponentResponse(
                component.ComponentId,
                message,
                201,
                null);
        }
        catch (DuplicateIdException ex)
        {
            return new CreateLearningComponentResponse(
                null,
                null,
                409,
                ex.Message);
        }
        catch (ValidationException ex)
        {
            return new CreateLearningComponentResponse(
                null,
                null,
                400,
                ex.Message);
        }
        catch (ArgumentException ex)
        {
            return new CreateLearningComponentResponse(
                null,
                null,
                400,
                ex.Message);
        }
    }
}

/// <summary>
/// Response object for the create learning component operation.
/// </summary>
/// <param name="ComponentId">The ID of the created component, or null if creation failed.</param>
/// <param name="Message">A success message, or null if creation failed.</param>
/// <param name="StatusCode">The HTTP status code representing the result.</param>
/// <param name="ErrorMessage">An error message if creation failed, or null if successful.</param>
public record class CreateLearningComponentResponse(
    string? ComponentId,
    string? Message,
    int StatusCode,
    string? ErrorMessage);
