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
    /// Gets the error message if the operation failed; otherwise, null.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the updated whiteboard if the operation succeeded; otherwise, null.
    /// </summary>
    public Whiteboard? Whiteboard { get; }

    private UpdateWhiteboardResult(bool isSuccess, string? errorMessage, Whiteboard? whiteboard)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Whiteboard = whiteboard;
    }

    /// <summary>
    /// Creates a successful result containing the updated whiteboard.
    /// </summary>
    /// <param name="whiteboard">The updated whiteboard entity.</param>
    /// <returns>A successful update result.</returns>
    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
        => new(true, null, whiteboard);

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed update result.</returns>
    public static UpdateWhiteboardResult Failure(string errorMessage)
        => new(false, errorMessage, null);
}
