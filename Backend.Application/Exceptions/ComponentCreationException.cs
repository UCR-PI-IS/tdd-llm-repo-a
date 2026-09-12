namespace UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

/// <summary>
/// Base exception for errors that occur during component creation.
/// </summary>
public abstract class ComponentCreationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentCreationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    protected ComponentCreationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Gets the category of component creation error.
    /// </summary>
    public abstract ComponentCreationErrorType ErrorType { get; }
}
