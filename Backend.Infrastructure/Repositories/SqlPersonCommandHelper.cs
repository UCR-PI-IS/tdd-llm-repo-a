using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

internal static class SqlPersonCommandHelper
{
    public static async Task AddAsync(UCRDatabaseContext dbContext, Person person)
    {
        await dbContext.Persons.AddAsync(person);
        await dbContext.SaveChangesAsync();
    }
}
