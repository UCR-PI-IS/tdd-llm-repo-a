using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning component.
/// Delegates to <see cref="ILearningComponentService"/> and maps results/errors to a response.
/// </summary>
public class CreateLearningComponentHandler : ICreateLearningComponentHandler
{
    private readonly ILearningComponentService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLearningComponentHandler"/> class.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    public CreateLearningComponentHandler(ILearningComponentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Handles the creation of a learning component using the injected service.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>A task containing the handler result.</returns>
    public Task<CreateLearningComponentResult> HandleAsync(CreateComponentRequest request)
    {
        return HandleAsync(_service, request);
    }

    /// <summary>
    /// Handles the creation of a learning component.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    /// <param name="request">The creation request.</param>
    /// <returns>A task containing the handler result.</returns>
    public static async Task<CreateLearningComponentResult> HandleAsync(
        ILearningComponentService service,
        CreateComponentRequest request)
    {
        try
        {
            var result = await service.CreateComponentAsync(request);
            return new CreateLearningComponentResult
            {
                ComponentId = result.ComponentId,
                Message = result.Message,
                StatusCode = 200
            };
        }
        catch (DuplicateIdException ex)
        {
            return new CreateLearningComponentResult
            {
                StatusCode = 409,
                ErrorMessage = ex.Message
            };
        }
        catch (ValidationException ex)
        {
            return new CreateLearningComponentResult
            {
                StatusCode = 400,
                ErrorMessage = ex.Message
            };
        }
    }
}
