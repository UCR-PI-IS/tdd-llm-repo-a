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
    private readonly ILearningSpaceReadRepository? _learningSpaceReadRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class
    /// with only the whiteboard repository.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    public WhiteboardService(IWhiteboardRepository whiteboardRepository)
    {
        _whiteboardRepository = whiteboardRepository;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class
    /// with both repositories.
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
    /// Creates a new whiteboard, validates it fits in the learning space, and persists it.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space does not exist.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard does not fit in the learning space.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        var learningSpace = await _learningSpaceReadRepository!.GetByIdAsync(request.LearningSpaceId);
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
    /// <param name="dto">The update DTO containing whiteboard parameters.</param>
    /// <returns>A result indicating success or failure of the update.</returns>
    public async Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto)
    {
        var existingWhiteboard = await _whiteboardRepository.GetByIdAsync(dto.WhiteboardId);
        if (existingWhiteboard == null)
            return UpdateWhiteboardResult.Failure("Whiteboard not found");

        try
        {
            if (_learningSpaceReadRepository != null)
            {
                var learningSpace = await _learningSpaceReadRepository.GetByIdAsync(existingWhiteboard.LearningSpaceId);
                if (learningSpace != null)
                {
                    existingWhiteboard.Update(
                        dto.Width, dto.Height, dto.Depth,
                        dto.X, dto.Y, dto.Z,
                        dto.Orientation, dto.MarkerColor,
                        learningSpace.Width, learningSpace.Length);
                }

                var existingComponents = await _whiteboardRepository.GetByLearningSpaceIdAsync(existingWhiteboard.LearningSpaceId)
                    ?? new List<Whiteboard>();
                existingWhiteboard.Update(
                    dto.Width, dto.Height, dto.Depth,
                    dto.X, dto.Y, dto.Z,
                    dto.Orientation, dto.MarkerColor,
                    existingComponents);
            }
            else
            {
                existingWhiteboard.Update(
                    dto.Width, dto.Height, dto.Depth,
                    dto.X, dto.Y, dto.Z,
                    dto.Orientation, dto.MarkerColor);
            }
        }
        catch (ArgumentException ex)
        {
            return UpdateWhiteboardResult.Failure(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return UpdateWhiteboardResult.Failure(ex.Message);
        }

        await _whiteboardRepository.UpdateAsync(existingWhiteboard);
        return UpdateWhiteboardResult.Success(existingWhiteboard);
    }
}
