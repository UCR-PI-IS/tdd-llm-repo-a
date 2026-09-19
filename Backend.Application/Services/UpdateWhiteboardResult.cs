using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Result object for whiteboard update operations.
/// </summary>
public class UpdateWhiteboardResult
{
    public bool IsSuccess { get; private set; }
    public Whiteboard? Value { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResult { IsSuccess = true, Value = whiteboard };
    }

    public static UpdateWhiteboardResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardResult { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
