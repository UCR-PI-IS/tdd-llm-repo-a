using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating and updating whiteboards.
/// </summary>
internal class WhiteboardService : IWhiteboardCreateService, IWhiteboardService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceReadRepository _learningSpaceReadRepository;
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
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
    /// Updates an existing whiteboard with the specified parameters.
    /// </summary>
    /// <param name="dto">The update parameters.</param>
    /// <returns>A result indicating success or failure of the update operation.</returns>
    public async Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto)
    {
        var whiteboard = await _whiteboardRepository.GetByIdAsync(dto.WhiteboardId);
        if (whiteboard == null)
            return UpdateWhiteboardResult.Failure("Whiteboard not found");

        if (string.IsNullOrEmpty(dto.MarkerColor))
            return UpdateWhiteboardResult.Failure("Invalid marker color");

        try
        {
            await ApplyUpdateWithValidation(whiteboard, dto);
        }
        catch (InvalidOperationException ex)
        {
            return UpdateWhiteboardResult.Failure(ex.Message);
        }

        await _whiteboardRepository.UpdateAsync(whiteboard);
        return UpdateWhiteboardResult.Success(whiteboard);
    }

    private async Task ApplyUpdateWithValidation(Whiteboard whiteboard, UpdateWhiteboardDto dto)
    {
        var learningSpace = await _learningSpaceRepository.GetByIdAsync(whiteboard.LearningSpaceId);
        var existingComponents = await _whiteboardRepository.GetByLearningSpaceIdAsync(whiteboard.LearningSpaceId);
        var otherComponents = FilterOtherComponents(existingComponents, whiteboard.ComponentId);

        if (learningSpace != null)
            whiteboard.Update(dto.Width, dto.Height, dto.Depth, dto.X, dto.Y, dto.Z, dto.Orientation, dto.MarkerColor, learningSpace.Width, learningSpace.Length);
        else
            whiteboard.Update(dto.Width, dto.Height, dto.Depth, dto.X, dto.Y, dto.Z, dto.Orientation, dto.MarkerColor);

        if (otherComponents.Count > 0)
            whiteboard.Update(dto.Width, dto.Height, dto.Depth, dto.X, dto.Y, dto.Z, dto.Orientation, dto.MarkerColor, otherComponents);
    }

    private static List<LearningComponent> FilterOtherComponents(List<LearningComponent>? components, string currentComponentId)
    {
        if (components == null)
            return new List<LearningComponent>();
        return components.Where(c => c.ComponentId != currentComponentId).ToList();
    }
}
