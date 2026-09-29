namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Base exception for building-related domain errors.
/// </summary>
public class BuildingDomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingDomainException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public BuildingDomainException(string message) : base(message)
    {
    }
}
