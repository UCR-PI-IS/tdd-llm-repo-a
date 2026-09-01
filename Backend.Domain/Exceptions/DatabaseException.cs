namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when a database operation fails.
/// </summary>
public class DatabaseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DatabaseException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}
