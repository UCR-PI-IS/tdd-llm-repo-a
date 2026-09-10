namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Base exception for application service layer errors.
/// </summary>
public class ApplicationServiceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationServiceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ApplicationServiceException(string message) : base(message)
    {
    }
}
