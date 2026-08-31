using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating whiteboards.
/// </summary>
internal class WhiteboardService : IWhiteboardCreateService
{
    private readonly ILearningSpaceListRepository _learningSpaceRepository;
    private readonly IWhiteboardRepository _whiteboardRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    public WhiteboardService(
        ILearningSpaceListRepository learningSpaceRepository,
        IWhiteboardRepository whiteboardRepository)
    {
        _learningSpaceRepository = learningSpaceRepository;
        _whiteboardRepository = whiteboardRepository;
    }

    /// <summary>
    /// Creates a new whiteboard in the specified learning space.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard details.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space does not exist.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard does not fit in the learning space.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        var learningSpace = await _learningSpaceRepository.GetByIdAsync(request.LearningSpaceId);
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
}
