using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a whiteboard update operation.
/// </summary>
public class UpdateWhiteboardResult
{
    /// <summary>
    /// Indicates whether the update was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message if the update failed.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// The updated whiteboard entity if successful.
    /// </summary>
    public Whiteboard? Whiteboard { get; }

    private UpdateWhiteboardResult(bool isSuccess, string errorMessage, Whiteboard? whiteboard)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Whiteboard = whiteboard;
    }

    /// <summary>
    /// Creates a successful update result.
    /// </summary>
    /// <param name="whiteboard">The updated whiteboard.</param>
    /// <returns>A success result.</returns>
    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResult(true, string.Empty, whiteboard);
    }

    /// <summary>
    /// Creates a failed update result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failure result.</returns>
    public static UpdateWhiteboardResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardResult(false, errorMessage, null);
    }
}
