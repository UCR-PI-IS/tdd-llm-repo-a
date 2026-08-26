using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL repository implementation for learning space operations.
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
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new learning space to the repository.
    /// </summary>
    /// <param name="learningSpace">The learning space to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(LearningSpace learningSpace)
    {
        if (learningSpace == null)
        {
            throw new ArgumentNullException(nameof(learningSpace));
        }

        _context.LearningSpaces.Add(learningSpace);
        await _context.SaveChangesAsync();
    }
}
