using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Helper for adding a university to the database.
/// </summary>
internal static class UniversityAddHelper
{
    public static async Task AddAsync(UCRDatabaseContext dbContext, University university)
    {
        await dbContext.Universities.AddAsync(university);
        await dbContext.SaveChangesAsync();
    }
}
