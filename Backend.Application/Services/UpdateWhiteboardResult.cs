using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the outcome of a whiteboard update operation.
/// </summary>
public class UpdateWhiteboardResult
{
    /// <summary>
    /// Indicates whether the update operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message when the operation fails; null when it succeeds.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// The updated whiteboard entity when the operation succeeds; null when it fails.
    /// </summary>
    public Whiteboard? Whiteboard { get; }

    private UpdateWhiteboardResult(bool isSuccess, string? errorMessage, Whiteboard? whiteboard)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Whiteboard = whiteboard;
    }

    /// <summary>
    /// Creates a successful result carrying the updated whiteboard.
    /// </summary>
    /// <param name="whiteboard">The updated whiteboard entity.</param>
    /// <returns>A successful <see cref="UpdateWhiteboardResult"/>.</returns>
    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
        => new(true, null, whiteboard);

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">Description of the failure.</param>
    /// <returns>A failed <see cref="UpdateWhiteboardResult"/>.</returns>
    public static UpdateWhiteboardResult Failure(string errorMessage)
        => new(false, errorMessage, null);
}
