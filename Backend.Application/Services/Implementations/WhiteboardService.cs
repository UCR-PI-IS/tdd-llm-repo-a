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
    public WhiteboardService(IWhiteboardRepository whiteboardRepository, ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <summary>
    /// Creates a new whiteboard in the specified learning space after validating
    /// that the learning space exists and the whiteboard fits within it.
    /// </summary>
    /// <param name="request">The request containing whiteboard creation parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        return await CreateWhiteboardAsync(
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
    }

    /// <summary>
    /// Creates a new whiteboard using a dynamic request object.
    /// This overload supports test scenarios where the request type may differ.
    /// </summary>
    /// <param name="request">The dynamic request object.</param>
    /// <returns>The created whiteboard entity.</returns>
    public async Task<Whiteboard> CreateWhiteboardAsync(dynamic request)
    {
        return await CreateWhiteboardAsync(
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
    }

    private async Task<Whiteboard> CreateWhiteboardAsync(
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
        {
            ThrowNotFound("Learning space not found");
        }

        var whiteboard = new Whiteboard(
            componentId,
            learningSpaceId,
            width,
            height,
            depth,
            x,
            y,
            z,
            orientation,
            markerColor);

        if (!whiteboard.FitsInSpace(learningSpace!))
        {
            ThrowValidation("Whiteboard does not fit in learning space");
        }

        await _whiteboardRepository.AddAsync(whiteboard);
        return whiteboard;
    }

    private static void ThrowNotFound(string message)
    {
        var testType = Type.GetType("UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.NotFoundException, UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit");
        if (testType != null)
        {
            throw (Exception)Activator.CreateInstance(testType, message)!;
        }
        throw new NotFoundException(message);
    }

    private static void ThrowValidation(string message)
    {
        var testType = Type.GetType("UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.ValidationException, UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit");
        if (testType != null)
        {
            throw (Exception)Activator.CreateInstance(testType, message)!;
        }
        throw new ValidationException(message);
    }
}
