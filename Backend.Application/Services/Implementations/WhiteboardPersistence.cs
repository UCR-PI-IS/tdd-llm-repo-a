using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Persists a whiteboard to the repository with exception wrapping.
/// </summary>
internal static class WhiteboardPersistence
{
    /// <summary>
    /// Saves a whiteboard to the database.
    /// </summary>
    /// <exception cref="DatabaseException">Thrown when the database operation fails.</exception>
    public static async Task SaveAsync(IWhiteboardRepository repository, Whiteboard whiteboard)
    {
        try
        {
            await repository.AddAsync(whiteboard);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Failed to save whiteboard to database: " + ex.Message, ex);
        }
    }
}
