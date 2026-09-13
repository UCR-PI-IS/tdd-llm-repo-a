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
    private readonly IComponentIdGenerator _idGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    public LearningComponentService(ILearningComponentRepository learningComponentRepository)
    {
        _learningComponentRepository = learningComponentRepository;
        _idGenerator = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="idGenerator">The component ID generator.</param>
    public LearningComponentService(ILearningComponentRepository learningComponentRepository, IComponentIdGenerator idGenerator)
    {
        _learningComponentRepository = learningComponentRepository;
        _idGenerator = idGenerator;
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
    /// Creates a new learning component with optional auto-generated ID.
    /// </summary>
    /// <param name="request">The request containing the component details.</param>
    /// <returns>The created learning component.</returns>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="DuplicateIdException">Thrown when a duplicate ID is provided.</exception>
    public async Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request)
    {
        ValidateRequest(request);

        string? componentId = request.componentId;
        if (string.IsNullOrEmpty(componentId))
        {
            do
            {
                componentId = await _idGenerator.GenerateIdAsync();
            } while (await _learningComponentRepository.ExistsAsync(componentId));
        }
        else
        {
            if (await _learningComponentRepository.ExistsAsync(componentId))
                throw new DuplicateIdException($"Component with ID {componentId} already exists");
        }

        var component = new LearningComponent(
            componentId,
            request.learningSpaceId,
            request.width,
            request.height,
            request.depth,
            request.x,
            request.y,
            request.z,
            request.orientation);

        await _learningComponentRepository.AddAsync(component);
        return component;
    }

    private static void ValidateRequest(CreateComponentRequest request)
    {
        if (request.width <= 0)
            throw new ValidationException("Width must be positive and non-zero");
        if (request.height < 0)
            throw new ValidationException("Height cannot be negative");
        if (request.depth < 0)
            throw new ValidationException("Depth cannot be negative");
        if (request.x < 0)
            throw new ValidationException("X cannot be negative");
        if (request.y < 0)
            throw new ValidationException("Y cannot be negative");
        if (request.z < 0)
            throw new ValidationException("Z cannot be negative");

        var validOrientations = new HashSet<string> { "North", "South", "East", "West" };
        if (!validOrientations.Contains(request.orientation))
            throw new ValidationException("Orientation must be North, South, East, or West");
    }
}
