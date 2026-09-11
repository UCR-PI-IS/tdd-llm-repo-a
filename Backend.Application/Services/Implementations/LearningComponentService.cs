using System.Reflection;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing learning component data.
/// </summary>
public class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _learningComponentRepository;
    private readonly IComponentIdGenerator _idGenerator;
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    /// <param name="idGenerator">The component ID generator.</param>
    public LearningComponentService(
        ILearningComponentRepository learningComponentRepository,
        IComponentIdGenerator idGenerator)
    {
        _learningComponentRepository = learningComponentRepository;
        _idGenerator = idGenerator;
    }

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
    /// <returns>The result containing the created component ID.</returns>
    public async Task<dynamic> CreateComponentAsync<TRequest>(TRequest request)
    {
        var (learningSpaceId, width, height, depth, x, y, z, orientation, componentId) = ExtractRequestData(request);

        ValidateDimensions(width, height, depth, x, y, z);
        ValidateOrientationValue(orientation);

        var finalComponentId = await ResolveComponentIdAsync(componentId);

        var learningComponent = new LearningComponent(
            finalComponentId,
            learningSpaceId,
            width,
            height,
            depth,
            x,
            y,
            z,
            orientation);

        await _learningComponentRepository.AddAsync(learningComponent);

        return new CreateComponentResult(finalComponentId);
    }

    private static (string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation, string? componentId) ExtractRequestData<TRequest>(TRequest request)
    {
        var requestType = typeof(TRequest);
        
        var learningSpaceId = (string)requestType.GetProperty("LearningSpaceId")!.GetValue(request)!;
        var width = (float)requestType.GetProperty("Width")!.GetValue(request)!;
        var height = (float)requestType.GetProperty("Height")!.GetValue(request)!;
        var depth = (float)requestType.GetProperty("Depth")!.GetValue(request)!;
        var x = (float)requestType.GetProperty("X")!.GetValue(request)!;
        var y = (float)requestType.GetProperty("Y")!.GetValue(request)!;
        var z = (float)requestType.GetProperty("Z")!.GetValue(request)!;
        var orientation = (string)requestType.GetProperty("Orientation")!.GetValue(request)!;
        var componentId = (string?)requestType.GetProperty("ComponentId")!.GetValue(request);

        return (learningSpaceId, width, height, depth, x, y, z, orientation, componentId);
    }

    private static void ValidateDimensions(float width, float height, float depth, float x, float y, float z)
    {
        if (width <= 0)
            throw new ValidationException("Width must be positive.");
        if (height < 0)
            throw new ValidationException("Height cannot be negative.");
        if (depth < 0)
            throw new ValidationException("Depth cannot be negative.");
        if (x < 0)
            throw new ValidationException("X coordinate cannot be negative.");
        if (y < 0)
            throw new ValidationException("Y coordinate cannot be negative.");
        if (z < 0)
            throw new ValidationException("Z coordinate cannot be negative.");
    }

    private static void ValidateOrientationValue(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ValidationException($"Invalid Orientation '{orientation}'. Must be one of: North, South, East, West.");
    }

    private async Task<string> ResolveComponentIdAsync(string? componentId)
    {
        if (!string.IsNullOrEmpty(componentId))
        {
            if (await _learningComponentRepository.ExistsAsync(componentId))
                throw new DuplicateIdException($"Component with ID '{componentId}' already exists.");

            return componentId;
        }

        string finalComponentId;
        do
        {
            finalComponentId = await _idGenerator.GenerateIdAsync();
        } while (await _learningComponentRepository.ExistsAsync(finalComponentId));

        return finalComponentId;
    }
}
