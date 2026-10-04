using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Generates unique component identifiers in the format "COMP-{guid}".
/// </summary>
internal class ComponentIdGenerator : IIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the generated unique identifier.</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid()}";
        return Task.FromResult(id);
    }
}
