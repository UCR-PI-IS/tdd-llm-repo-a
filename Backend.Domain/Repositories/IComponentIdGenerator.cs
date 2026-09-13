namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for generating unique component IDs.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component ID asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing the generated ID.</returns>
    Task<string> GenerateIdAsync();
}
