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
    private readonly IIdGenerator _idGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="idGenerator">The ID generator dependency.</param>
    public LearningComponentService(
        ILearningComponentRepository learningComponentRepository,
        IIdGenerator idGenerator)
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
    /// Creates a new learning component with the specified parameters.
    /// If no ComponentId is provided, a unique ID is auto-generated.
    /// </summary>
    /// <param name="request">The creation request containing component parameters.</param>
    /// <returns>The created learning component.</returns>
    /// <exception cref="ValidationException">Thrown when the request contains invalid data.</exception>
    /// <exception cref="DuplicateIdException">Thrown when an explicit ID already exists.</exception>
    public async Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request)
    {
        ValidateRequest(request);

        var componentId = await ResolveComponentIdAsync(request.ComponentId);

        var component = new LearningComponent(
            componentId,
            request.LearningSpaceId,
            request.Width, request.Height, request.Depth,
            request.X, request.Y, request.Z,
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
        {
            throw new DuplicateIdException(
                $"Component with ID '{requestedId}' already exists");
        }

        return requestedId;
    }

    private static void ValidateRequest(CreateComponentRequest request)
    {
        ThrowIfNotPositive(request.Width, "Width");
        ThrowIfNegative(request.Height, "Height");
        ThrowIfNegative(request.Depth, "Depth");
        ThrowIfNegative(request.X, "X");
        ThrowIfNegative(request.Y, "Y");
        ThrowIfNegative(request.Z, "Z");
        ValidateOrientation(request.Orientation);
    }

    private static void ThrowIfNotPositive(float value, string fieldName)
    {
        if (value <= 0f)
            throw new ValidationException($"{fieldName} must be positive");
    }

    private static void ThrowIfNegative(float value, string fieldName)
    {
        if (value < 0f)
            throw new ValidationException($"{fieldName} cannot be negative");
    }

    private static void ValidateOrientation(string orientation)
    {
        if (orientation is not ("North" or "South" or "East" or "West"))
            throw new ValidationException(
                "Orientation must be one of: North, South, East, West");
    }
}
