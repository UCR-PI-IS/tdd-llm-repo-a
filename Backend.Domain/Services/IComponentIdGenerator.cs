namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

/// <summary>
/// Contract for generating unique identifiers for learning components.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the generated identifier.</returns>
    Task<string> GenerateIdAsync();
}
