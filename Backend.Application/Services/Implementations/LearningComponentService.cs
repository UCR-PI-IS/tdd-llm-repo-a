using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing learning component data.
/// </summary>
internal class LearningComponentService : ILearningComponentService
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    private readonly ILearningComponentRepository _learningComponentRepository;
    private readonly IComponentIdGenerator _idGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="idGenerator">The component ID generator dependency.</param>
    public LearningComponentService(
        ILearningComponentRepository learningComponentRepository,
        IComponentIdGenerator idGenerator)
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
    /// Creates a new learning component, auto-generating an ID if none is provided.
    /// </summary>
    /// <param name="request">The creation request containing component data.</param>
    /// <returns>The created learning component entity.</returns>
    /// <exception cref="ValidationException">Thrown when the request data is invalid.</exception>
    /// <exception cref="DuplicateIdException">Thrown when an explicit ID already exists.</exception>
    public async Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request)
    {
        ValidateCreateRequest(request);

        var componentId = await ResolveComponentIdAsync(request.ComponentId);

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

    private async Task<string> ResolveComponentIdAsync(string? requestedId)
    {
        if (string.IsNullOrEmpty(requestedId))
        {
            string generatedId;
            do
            {
                generatedId = await _idGenerator.GenerateIdAsync();
            }
            while (await _learningComponentRepository.ExistsAsync(generatedId));
            return generatedId;
        }

        if (await _learningComponentRepository.ExistsAsync(requestedId))
            throw new DuplicateIdException($"Component with ID '{requestedId}' already exists.");
        return requestedId;
    }

    private static void ValidateCreateRequest(CreateComponentRequest request)
    {
        if (request.Width <= 0f)
            throw new ValidationException("Width must be positive.");
        ValidateNonNegative(request.Height, nameof(request.Height));
        ValidateNonNegative(request.Depth, nameof(request.Depth));
        ValidateNonNegative(request.X, nameof(request.X));
        ValidateNonNegative(request.Y, nameof(request.Y));
        ValidateNonNegative(request.Z, nameof(request.Z));
        if (!ValidOrientations.Contains(request.Orientation))
            throw new ValidationException("Orientation must be one of: North, South, East, West.");
    }

    private static void ValidateNonNegative(float value, string name)
    {
        if (value < 0f)
            throw new ValidationException($"{name} cannot be negative.");
    }
}
