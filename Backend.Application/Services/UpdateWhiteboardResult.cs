using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a whiteboard update operation.
/// </summary>
public class UpdateWhiteboardResult
{
    /// <summary>
    /// Gets a value indicating whether the update operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the update operation failed.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the updated whiteboard entity if the operation was successful.
    /// </summary>
    public Whiteboard? Whiteboard { get; }

    private UpdateWhiteboardResult(bool isSuccess, string errorMessage, Whiteboard? whiteboard)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Whiteboard = whiteboard;
    }

    /// <summary>
    /// Creates a successful update result with the updated whiteboard.
    /// </summary>
    /// <param name="whiteboard">The updated whiteboard entity.</param>
    /// <returns>A successful update result.</returns>
    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResult(true, string.Empty, whiteboard);
    }

    /// <summary>
    /// Creates a failed update result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing why the update failed.</param>
    /// <returns>A failed update result.</returns>
    public static UpdateWhiteboardResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardResult(false, errorMessage, null);
    }
}
