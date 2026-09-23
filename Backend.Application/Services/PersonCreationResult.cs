namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a person creation operation.
/// </summary>
/// <param name="IsSuccess">Whether the operation succeeded.</param>
/// <param name="ErrorMessage">The error message if the operation failed; null if it succeeded.</param>
public record PersonCreationResult(bool IsSuccess, string? ErrorMessage);
