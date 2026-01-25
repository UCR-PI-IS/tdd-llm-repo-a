using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing LearningComponentDetail entities.
/// </summary>
internal class LearningComponentDetailRepository : ILearningComponentDetailRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentDetailRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public LearningComponentDetailRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task CreateAsync(LearningComponentDetail detail)
    {
        await _context.LearningComponentDetails.AddAsync(detail);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<LearningComponentDetail?> GetByIdAsync(Guid detailId)
    {
        return await _context.LearningComponentDetails
            .Include(lcd => lcd.Component)
            .FirstOrDefaultAsync(lcd => lcd.DetailId == detailId);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LearningComponentDetail>> GetByComponentIdAsync(Guid componentId)
    {
        return await _context.LearningComponentDetails
            .Include(lcd => lcd.Component)
            .Where(lcd => lcd.ComponentId == componentId)
            .ToListAsync();
    }
}
