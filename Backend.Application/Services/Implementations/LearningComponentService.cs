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
    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="idGenerator">The ID generator for creating unique component IDs.</param>
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
    /// Creates a new learning component.
    /// </summary>
    /// <param name="request">The request containing component data.</param>
    /// <returns>The created learning component.</returns>
    /// <exception cref="ValidationException">Thrown when the request data is invalid.</exception>
    /// <exception cref="DuplicateIdException">Thrown when a component with the same ID already exists.</exception>
    public async Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request)
    {
        ValidateRequest(request);

        var componentId = await ResolveComponentIdAsync(request.ComponentId);
        var component = CreateComponentFromRequest(request, componentId);

        await _learningComponentRepository.AddAsync(component);
        return component;
    }

    private static void ValidateRequest(CreateComponentRequest request)
    {
        ValidateDimensions(request);
        ValidateOrientation(request.Orientation);
    }

    private static void ValidateDimensions(CreateComponentRequest request)
    {
        if (request.Width <= 0)
            throw new ValidationException("Width must be positive.");
        if (request.Height < 0)
            throw new ValidationException("Height cannot be negative.");
        if (request.Depth < 0)
            throw new ValidationException("Depth cannot be negative.");
        if (request.X < 0)
            throw new ValidationException("X coordinate cannot be negative.");
        if (request.Y < 0)
            throw new ValidationException("Y coordinate cannot be negative.");
        if (request.Z < 0)
            throw new ValidationException("Z coordinate cannot be negative.");
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ValidationException($"Orientation '{orientation}' is invalid. Must be one of: North, South, East, West.");
    }

    private async Task<string> ResolveComponentIdAsync(string? requestedId)
    {
        if (string.IsNullOrEmpty(requestedId))
        {
            return await GenerateUniqueIdAsync();
        }

        if (await _learningComponentRepository.ExistsAsync(requestedId))
        {
            throw new DuplicateIdException($"A component with ID '{requestedId}' already exists.");
        }

        return requestedId;
    }

    private static LearningComponent CreateComponentFromRequest(CreateComponentRequest request, string componentId)
    {
        return new LearningComponent(
            componentId,
            request.LearningSpaceId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation);
    }

    /// <summary>
    /// Generates a unique component ID, retrying if the generated ID already exists.
    /// </summary>
    /// <returns>A unique component ID.</returns>
    private async Task<string> GenerateUniqueIdAsync()
    {
        string generatedId;
        do
        {
            generatedId = await _idGenerator.GenerateIdAsync();
        } while (await _learningComponentRepository.ExistsAsync(generatedId));

        return generatedId;
    }
}
