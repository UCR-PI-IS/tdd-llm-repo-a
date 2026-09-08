using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing learning component data.
/// </summary>
internal class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _learningComponentRepository;
    private readonly IComponentIdGenerator _componentIdGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="componentIdGenerator">The component ID generator dependency.</param>
    public LearningComponentService(
        ILearningComponentRepository learningComponentRepository,
        IComponentIdGenerator componentIdGenerator)
    {
        _learningComponentRepository = learningComponentRepository;
        _componentIdGenerator = componentIdGenerator;
    }

    /// <summary>
    /// Retrieves all learning components that belong to the specified learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is null or empty.</exception>
    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));

        return _learningComponentRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }

    /// <summary>
    /// Creates a new learning component. If no ID is provided, a unique ID is auto-generated.
    /// </summary>
    /// <param name="request">The request containing the component data.</param>
    /// <returns>The created learning component entity.</returns>
    /// <exception cref="ValidationException">Thrown when the request data is invalid.</exception>
    /// <exception cref="DuplicateIdException">Thrown when an explicit ID already exists.</exception>
    public async Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request)
    {
        ComponentRequestValidator.Validate(request);

        string componentId;
        if (!string.IsNullOrEmpty(request.ComponentId))
        {
            componentId = request.ComponentId;
            if (await _learningComponentRepository.ExistsAsync(componentId))
                throw new DuplicateIdException($"Component with ID '{componentId}' already exists.");
        }
        else
        {
            do
            {
                componentId = await _componentIdGenerator.GenerateIdAsync();
            } while (await _learningComponentRepository.ExistsAsync(componentId));
        }

        var component = new LearningComponent(
            componentId,
            request.LearningSpaceId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation);

        await _learningComponentRepository.AddAsync(component);

        return component;
    }

}
