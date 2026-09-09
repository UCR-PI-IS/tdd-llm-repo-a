namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for generating unique component identifiers.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    Task<string> GenerateIdAsync();
}
