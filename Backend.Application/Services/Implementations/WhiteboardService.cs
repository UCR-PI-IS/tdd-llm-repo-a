using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for creating whiteboards.
/// </summary>
internal class WhiteboardService : IWhiteboardService
{
    private readonly CreateWhiteboardUseCase _useCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        _useCase = new CreateWhiteboardUseCase(whiteboardRepository, learningSpaceRepository);
    }

    /// <summary>
    /// Creates a new whiteboard with the specified parameters and persists it.
    /// </summary>
    /// <param name="request">The request containing whiteboard creation parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space is not found.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard doesn't fit in the learning space.</exception>
    /// <exception cref="DatabaseException">Thrown when the database operation fails.</exception>
    public Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        return _useCase.ExecuteAsync(request);
    }
}
