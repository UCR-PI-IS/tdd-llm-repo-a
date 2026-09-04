using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that creates whiteboards.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard with the specified parameters.
    /// </summary>
    /// <param name="request">The request containing whiteboard creation parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request);
}

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a database operation fails.
/// </summary>
public class DatabaseException : Exception
{
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}
