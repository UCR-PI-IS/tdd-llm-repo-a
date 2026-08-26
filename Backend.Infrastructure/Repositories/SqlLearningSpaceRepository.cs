using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL implementation of the learning space repository.
/// </summary>
internal class SqlLearningSpaceRepository : ILearningSpaceRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningSpaceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SqlLearningSpaceRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new learning space to the repository.
    /// </summary>
    /// <param name="learningSpace">The learning space to add.</param>
    public async Task AddAsync(LearningSpace learningSpace)
    {
        _context.LearningSpaces.Add(learningSpace);
        await _context.SaveChangesAsync();
    }
}
