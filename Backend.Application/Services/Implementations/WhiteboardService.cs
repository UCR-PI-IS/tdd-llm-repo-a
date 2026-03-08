using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for whiteboard operations.
/// </summary>
public class WhiteboardService : IWhiteboardService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the WhiteboardService class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository</param>
    /// <param name="learningSpaceRepository">The learning space repository</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <inheritdoc/>
    public async Task<Result> CreateWhiteboardAsync(string whiteboardId, double width, double height, string learningSpaceId)
    {
        try
        {
            // Get the learning space
            var learningSpace = await _learningSpaceRepository.GetByIdAsync(learningSpaceId);
            if (learningSpace == null)
            {
                return Result.Failure("Learning space not found");
            }

            // Create the whiteboard (this will validate dimensions)
            var whiteboard = new Whiteboard(whiteboardId, width, height);

            // Check if whiteboard fits in the learning space
            if (!learningSpace.CanFitWhiteboard(whiteboard))
            {
                return Result.Failure("Whiteboard doesn't fit in learning space");
            }

            // Add the whiteboard
            await _whiteboardRepository.AddAsync(whiteboard);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
