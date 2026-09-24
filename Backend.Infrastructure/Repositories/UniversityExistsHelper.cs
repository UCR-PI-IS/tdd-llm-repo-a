using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Helper for checking university existence in the database.
/// </summary>
internal static class UniversityExistsHelper
{
    public static Task<bool> ExistsByNameAsync(UCRDatabaseContext dbContext, string name)
    {
        foreach (var university in dbContext.Universities)
        {
            if (university.Name == name)
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
