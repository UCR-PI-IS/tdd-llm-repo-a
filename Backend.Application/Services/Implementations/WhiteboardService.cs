using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for whiteboard operations including creation and updates.
/// </summary>
internal class WhiteboardService : IWhiteboardCreateService, IWhiteboardService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceReadRepository? _learningSpaceReadRepository;
    private readonly ILearningSpaceRepository? _learningSpaceRepository;

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
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class
    /// with only the whiteboard repository (for simple updates without space validation).
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    public WhiteboardService(IWhiteboardRepository whiteboardRepository)
    {
        _whiteboardRepository = whiteboardRepository;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class
    /// with whiteboard repository and learning space repository (for updates with space validation).
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
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class
    /// with all dependencies. This constructor is used by dependency injection.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceReadRepository">The learning space read repository dependency.</param>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceReadRepository learningSpaceReadRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceReadRepository = learningSpaceReadRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <summary>
    /// Creates a new whiteboard, validates it fits in the learning space, and persists it.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    public Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        return CreateWhiteboardHelper.ExecuteAsync(request, _whiteboardRepository, _learningSpaceReadRepository!);
    }

    /// <summary>
    /// Updates an existing whiteboard with the specified parameters.
    /// </summary>
    /// <param name="dto">The update DTO containing whiteboard parameters.</param>
    /// <returns>A result indicating success or failure of the update operation.</returns>
    public Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto)
    {
        return UpdateWhiteboardHelper.ExecuteAsync(dto, _whiteboardRepository, _learningSpaceRepository);
    }
}
