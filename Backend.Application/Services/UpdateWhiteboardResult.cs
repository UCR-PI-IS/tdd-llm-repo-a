using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a whiteboard update operation.
/// </summary>
public class UpdateWhiteboardResult
{
    /// <summary>
    /// Gets a value indicating whether the update was successful.
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Gets the error message if the update failed; otherwise, null.
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// Gets the updated whiteboard if the update was successful; otherwise, null.
    /// </summary>
    public Whiteboard? Whiteboard { get; private init; }

    /// <summary>
    /// Creates a successful result containing the updated whiteboard.
    /// </summary>
    /// <param name="whiteboard">The updated whiteboard entity.</param>
    /// <returns>A successful <see cref="UpdateWhiteboardResult"/>.</returns>
    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResult
        {
            IsSuccess = true,
            Whiteboard = whiteboard
        };
    }

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed <see cref="UpdateWhiteboardResult"/>.</returns>
    public static UpdateWhiteboardResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
