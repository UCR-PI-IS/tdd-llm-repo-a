using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

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
        CreateComponentRequestValidator.ValidateLearningSpaceId(learningSpaceId);
        return _learningComponentRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }

    /// <summary>
    /// Creates a new learning component, generating a unique ID if none is provided.
    /// </summary>
    /// <param name="request">The creation request containing component data.</param>
    /// <returns>A result containing the created component's ID and a confirmation message.</returns>
    public async Task<CreateComponentResult> CreateComponentAsync(CreateComponentRequest request)
    {
        CreateComponentRequestValidator.Validate(request);

        string componentId;
        if (string.IsNullOrEmpty(request.ComponentId))
        {
            do
            {
                componentId = await _componentIdGenerator.GenerateIdAsync();
            }
            while (await _learningComponentRepository.ExistsAsync(componentId));
        }
        else
        {
            componentId = request.ComponentId;
            if (await _learningComponentRepository.ExistsAsync(componentId))
                throw new DuplicateIdException($"Component with ID '{componentId}' already exists.");
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

        return new CreateComponentResult
        {
            ComponentId = componentId,
            Message = string.IsNullOrEmpty(request.ComponentId)
                ? "Component created with auto-generated ID"
                : "Component created successfully"
        };
    }
}
