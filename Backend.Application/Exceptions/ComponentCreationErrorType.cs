namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Categorizes the type of error that occurred during component creation.
/// </summary>
public enum ComponentCreationErrorType
{
    /// <summary>
    /// A validation error (e.g., invalid dimensions or orientation).
    /// </summary>
    ValidationError,

    /// <summary>
    /// The component ID already exists.
    /// </summary>
    DuplicateIdError,

    /// <summary>
    /// An unexpected error that does not match known categories.
    /// </summary>
    UnexpectedError
}