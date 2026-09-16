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
    /// Creates a new learning component, generating a unique ID if none is provided.
    /// </summary>
    /// <param name="request">The creation request containing component parameters.</param>
    /// <returns>The created learning component entity.</returns>
    /// <exception cref="ValidationException">Thrown when the request contains invalid data.</exception>
    /// <exception cref="DuplicateIdException">Thrown when the provided ID already exists.</exception>
    public async Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request)
    {
        ValidateRequest(request);

        string componentId;

        if (string.IsNullOrEmpty(request.ComponentId))
        {
            // Auto-generate ID, retrying if it already exists
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
            {
                throw new DuplicateIdException(
                    $"Component with ID '{componentId}' already exists.");
            }
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

    private static void ValidateRequest(CreateComponentRequest request)
    {
        EnsurePositive(request.Width, "Width");
        EnsureNonNegative(request.Height, "Height");
        EnsureNonNegative(request.Depth, "Depth");
        EnsureNonNegative(request.X, "X");
        EnsureNonNegative(request.Y, "Y");
        EnsureNonNegative(request.Z, "Z");
        EnsureValidOrientation(request.Orientation);
    }

    private static void EnsurePositive(float value, string name)
    {
        if (value <= 0f)
            throw new ValidationException($"{name} must be positive.");
    }

    private static void EnsureNonNegative(float value, string name)
    {
        if (value < 0f)
            throw new ValidationException($"{name} cannot be negative.");
    }

    private static void EnsureValidOrientation(string orientation)
    {
        if (orientation is not ("North" or "South" or "East" or "West"))
            throw new ValidationException(
                $"Invalid Orientation '{orientation}'. Must be one of: North, South, East, West.");
    }
}
