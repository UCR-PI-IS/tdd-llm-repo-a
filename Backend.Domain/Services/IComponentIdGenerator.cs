namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

/// <summary>
/// Generates unique identifiers for learning components.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component ID asynchronously.
    /// </summary>
    /// <returns>A unique component identifier string.</returns>
    Task<string> GenerateIdAsync();
}
