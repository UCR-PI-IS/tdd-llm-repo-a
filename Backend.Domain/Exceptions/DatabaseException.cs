namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Exception thrown when a database operation fails.
/// </summary>
public class DatabaseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DatabaseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
