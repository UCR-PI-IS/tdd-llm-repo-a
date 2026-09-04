using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for creating whiteboards.
/// </summary>
public class WhiteboardService : IWhiteboardService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    public WhiteboardService(IWhiteboardRepository whiteboardRepository, ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <summary>
    /// Creates a new whiteboard with the specified parameters and persists it.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space is not found.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard doesn't fit in the learning space.</exception>
    /// <exception cref="DatabaseException">Thrown when the database operation fails.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        var learningSpace = await _learningSpaceRepository.GetByIdAsync(request.LearningSpaceId);
        if (learningSpace == null)
        {
            throw new NotFoundException("Learning space not found");
        }

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
        {
            throw new ValidationException("Whiteboard does not fit in the learning space");
        }

        try
        {
            await _whiteboardRepository.AddAsync(whiteboard);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }

        return whiteboard;
    }
}
