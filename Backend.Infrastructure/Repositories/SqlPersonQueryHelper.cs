using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

internal static class SqlPersonQueryHelper
{
    public static Task<bool> ExistsByEmailAsync(UCRDatabaseContext dbContext, string email)
    {
        return Task.FromResult(dbContext.Persons.Any(p => p.Email == email));
    }

    public static Task<bool> ExistsByIdentityNumberAsync(UCRDatabaseContext dbContext, string identityNumber)
    {
        return Task.FromResult(dbContext.Persons.Any(p => p.IdentityNumber == identityNumber));
    }
}
