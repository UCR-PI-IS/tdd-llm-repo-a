using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing LearningComponent entities.
/// </summary>
internal class LearningComponentRepository : ILearningComponentRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public LearningComponentRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId)
    {
        return await _context.LearningComponents
            .Where(lc => lc.LearningSpaceId == learningSpaceId)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task CreateAsync(LearningComponent component)
    {
        await _context.LearningComponents.AddAsync(component);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<LearningComponent?> GetByIdAsync(Guid componentId)
    {
        return await _context.LearningComponents
            .FirstOrDefaultAsync(lc => lc.ComponentId == componentId);
    }
}
