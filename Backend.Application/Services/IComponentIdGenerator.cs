namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for generating unique component identifiers.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier asynchronously.
    /// </summary>
    /// <returns>A unique component identifier string.</returns>
    Task<string> GenerateIdAsync();
}
