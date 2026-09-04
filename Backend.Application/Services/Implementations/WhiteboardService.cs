using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for whiteboard operations.
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
    /// Creates a new whiteboard in the specified learning space.
    /// </summary>
    /// <param name="request">The request containing whiteboard creation parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space is not found.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard doesn't fit in the learning space.</exception>
    /// <exception cref="DatabaseException">Thrown when the database operation fails.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        // Check if learning space exists
        var learningSpace = await _learningSpaceRepository.GetByIdAsync(request.LearningSpaceId);
        if (learningSpace == null)
        {
            throw new NotFoundException("Learning space not found");
        }

        // Create the whiteboard entity
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

        // Check if whiteboard fits in the learning space
        if (!whiteboard.FitsInSpace(learningSpace))
        {
            throw new ValidationException("Whiteboard does not fit in learning space");
        }

        // Save the whiteboard
        try
        {
            await _whiteboardRepository.AddAsync(whiteboard);
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Failed to save whiteboard", ex);
        }

        return whiteboard;
    }
}
