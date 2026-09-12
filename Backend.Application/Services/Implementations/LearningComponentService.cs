using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for managing learning component data.
/// </summary>
internal class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _learningComponentRepository;
    private readonly IComponentIdGenerator? _idGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="idGenerator">Optional ID generator for auto-generating component IDs.</param>
    public LearningComponentService(
        ILearningComponentRepository learningComponentRepository,
        IComponentIdGenerator? idGenerator = null)
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
    /// Creates a new learning component with automatic or explicit ID assignment.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>A response containing the created component's identifier.</returns>
    /// <exception cref="DuplicateIdException">Thrown when an explicit ID already exists.</exception>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public async Task<CreateComponentResponse> CreateComponentAsync(CreateComponentRequest request)
    {
        string componentId;

        if (!string.IsNullOrEmpty(request.ComponentId))
        {
            componentId = request.ComponentId;
            if (await _learningComponentRepository.ExistsAsync(componentId))
            {
                throw new DuplicateIdException($"Component with ID '{componentId}' already exists");
            }
        }
        else
        {
            if (_idGenerator == null)
            {
                throw new InvalidOperationException("ID generator is not configured.");
            }

            do
            {
                componentId = await _idGenerator.GenerateIdAsync();
            }
            while (await _learningComponentRepository.ExistsAsync(componentId));
        }

        try
        {
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
            return new CreateComponentResponse(componentId);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }
}
