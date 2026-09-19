using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of an update whiteboard operation.
/// </summary>
public class UpdateWhiteboardResult
{
    /// <summary>
    /// Indicates whether the update operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the updated whiteboard if the operation succeeded.
    /// </summary>
    public Whiteboard? Whiteboard { get; }

    private UpdateWhiteboardResult(bool isSuccess, string? errorMessage, Whiteboard? whiteboard)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Whiteboard = whiteboard;
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <param name="whiteboard">The updated whiteboard.</param>
    /// <returns>A success result.</returns>
    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResult(true, null, whiteboard);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failure result.</returns>
    public static UpdateWhiteboardResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardResult(false, errorMessage, null);
    }
}
