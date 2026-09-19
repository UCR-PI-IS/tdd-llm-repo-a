using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating and updating whiteboards.
/// </summary>
internal class WhiteboardService : IWhiteboardCreateService, IWhiteboardUpdateService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceReadRepository _learningSpaceReadRepository;
    private readonly ILearningComponentRepository? _learningComponentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceReadRepository">The learning space read repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceReadRepository learningSpaceReadRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceReadRepository = learningSpaceReadRepository;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceReadRepository">The learning space read repository dependency.</param>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceReadRepository learningSpaceReadRepository,
        ILearningComponentRepository learningComponentRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceReadRepository = learningSpaceReadRepository;
        _learningComponentRepository = learningComponentRepository;
    }

    /// <summary>
    /// Creates a new whiteboard, validates it fits in the learning space, and persists it.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space does not exist.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard does not fit in the learning space.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        var learningSpace = await _learningSpaceReadRepository.GetByIdAsync(request.LearningSpaceId);
        if (learningSpace == null)
            throw new NotFoundException("Learning space not found");

        var whiteboard = new Whiteboard(
            request.ComponentId,
            request.LearningSpaceId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation,
            request.MarkerColor);

        if (!whiteboard.FitsInSpace(learningSpace))
            throw new ValidationException("Whiteboard does not fit in the learning space");

        await _whiteboardRepository.AddAsync(whiteboard);
        return whiteboard;
    }

    /// <summary>
    /// Updates an existing whiteboard, validates the new values, and persists changes.
    /// </summary>
    /// <param name="request">The update request containing whiteboard parameters.</param>
    /// <returns>The result of the update operation.</returns>
    public async Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(dynamic request)
    {
        string whiteboardId = request.WhiteboardId;
        float width = request.Width;
        float height = request.Height;
        float depth = request.Depth;
        float x = request.X;
        float y = request.Y;
        float z = request.Z;
        string orientation = request.Orientation;
        string markerColor = request.MarkerColor;

        var existingWhiteboard = await _whiteboardRepository.GetByIdAsync(whiteboardId);
        if (existingWhiteboard == null)
        {
            return UpdateWhiteboardResult.Failure("Whiteboard not found");
        }

        if (string.IsNullOrEmpty(markerColor))
        {
            return UpdateWhiteboardResult.Failure("Invalid marker color");
        }

        var learningSpace = await _learningSpaceReadRepository.GetByIdAsync(existingWhiteboard.LearningSpaceId);
        if (learningSpace == null)
        {
            return UpdateWhiteboardResult.Failure("Learning space not found");
        }

        if (Math.Max(x + width - learningSpace.Width, z + depth - learningSpace.Length) > 0)
        {
            return UpdateWhiteboardResult.Failure("Position exceeds learning space boundaries");
        }

        if (_learningComponentRepository != null)
        {
            var existingComponents = await _learningComponentRepository.GetComponentsByLearningSpaceIdAsync(existingWhiteboard.LearningSpaceId);
            var otherComponents = existingComponents.Where(c => c.ComponentId != whiteboardId);
            if (otherComponents.Any(c => Whiteboard.Overlaps(x, y, z, width, height, depth,
                c.X, c.Y, c.Z, c.Width, c.Height, c.Depth)))
            {
                return UpdateWhiteboardResult.Failure("Position overlaps with existing component");
            }
        }

        try
        {
            existingWhiteboard.Update(width, height, depth, x, y, z, orientation, markerColor);
        }
        catch (ArgumentException ex) when (ex.ParamName == "markerColor")
        {
            return UpdateWhiteboardResult.Failure("Invalid marker color");
        }
        catch (InvalidOperationException ex)
        {
            return UpdateWhiteboardResult.Failure(ex.Message);
        }

        await _whiteboardRepository.UpdateAsync(existingWhiteboard);
        return UpdateWhiteboardResult.Success(existingWhiteboard);
    }
}
