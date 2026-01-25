using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for LearningComponent entity operations.
/// </summary>
internal class LearningComponentRepository : ILearningComponentRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Constructor for LearningComponentRepository.
    /// </summary>
    /// <param name="context">The database context</param>
    public LearningComponentRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all learning components associated with a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space</param>
    /// <returns>A collection of learning components in the specified learning space</returns>
    public async Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId)
    {
        return await _context.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId)
            .ToListAsync();
    }
}
