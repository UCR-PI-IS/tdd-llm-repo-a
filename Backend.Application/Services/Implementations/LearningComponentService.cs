using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing learning components.
/// </summary>
internal class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _learningComponentRepository;
    private readonly IComponentIdGenerator? _idGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    public LearningComponentService(ILearningComponentRepository learningComponentRepository)
    {
        _learningComponentRepository = learningComponentRepository;
        _idGenerator = null;
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
    /// Creates a new learning component with an auto-generated or explicit ID.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>The result of the creation operation.</returns>
    public async Task<CreateComponentResult> CreateComponentAsync(CreateComponentRequest request)
    {
        ValidateRequest(request);

        string componentId;
        string message;

        if (!string.IsNullOrEmpty(request.ComponentId))
        {
            // Explicit ID provided - check for duplicates
            if (await _learningComponentRepository.ExistsAsync(request.ComponentId))
                throw new DuplicateIdException($"Component ID '{request.ComponentId}' already exists.");

            componentId = request.ComponentId;
            message = "Component created with explicit ID";
        }
        else
        {
            // Auto-generate ID with retry for uniqueness
            if (_idGenerator == null)
                throw new InvalidOperationException("ID generator is not configured.");

            do
            {
                componentId = await _idGenerator.GenerateIdAsync();
            }
            while (await _learningComponentRepository.ExistsAsync(componentId));

            message = "Component created with auto-generated ID";
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
            Message = message
        };
    }

    private static void ValidateRequest(CreateComponentRequest request)
    {
        if (request.Width <= 0)
            throw new ValidationException("Width must be positive.");
        if (request.Height <= 0)
            throw new ValidationException("Height must be positive.");
        if (request.Depth < 0)
            throw new ValidationException("Depth cannot be negative.");
        if (request.X < 0)
            throw new ValidationException("X cannot be negative.");
        if (request.Y < 0)
            throw new ValidationException("Y cannot be negative.");
        if (request.Z < 0)
            throw new ValidationException("Z cannot be negative.");

        var validOrientations = new HashSet<string> { "North", "South", "East", "West" };
        if (!validOrientations.Contains(request.Orientation))
            throw new ValidationException("Orientation must be one of: North, South, East, West.");
    }
}
