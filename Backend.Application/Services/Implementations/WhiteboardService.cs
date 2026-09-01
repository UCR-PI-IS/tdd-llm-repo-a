using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating whiteboards.
/// </summary>
internal class WhiteboardService : IWhiteboardService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <summary>
    /// Creates a new whiteboard with the specified parameters and persists it.
    /// </summary>
    /// <param name="componentId">Unique identifier for the whiteboard.</param>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <param name="width">Width of the whiteboard in meters.</param>
    /// <param name="height">Height of the whiteboard in meters.</param>
    /// <param name="depth">Depth of the whiteboard in meters.</param>
    /// <param name="x">X coordinate position.</param>
    /// <param name="y">Y coordinate position.</param>
    /// <param name="z">Z coordinate position.</param>
    /// <param name="orientation">Orientation of the whiteboard.</param>
    /// <param name="markerColor">Marker color of the whiteboard.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space does not exist.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard does not fit in the learning space.</exception>
    /// <exception cref="DatabaseException">Thrown when the database operation fails.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor)
    {
        var learningSpace = await _learningSpaceRepository.GetByIdAsync(learningSpaceId);
        if (learningSpace == null)
            throw new NotFoundException("Learning space not found");

        var whiteboard = new Whiteboard(
            componentId, learningSpaceId,
            width, height, depth,
            x, y, z,
            orientation, markerColor);

        if (!whiteboard.FitsInSpace(learningSpace))
            throw new ValidationException("Whiteboard does not fit in the learning space");

        try
        {
            await _whiteboardRepository.AddAsync(whiteboard);
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new DatabaseException("DB error", ex);
        }

        return whiteboard;
    }
}
