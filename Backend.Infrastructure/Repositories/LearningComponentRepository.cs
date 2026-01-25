using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

public class LearningComponentRepository : ILearningComponentRepository
{
    private readonly UCRDatabaseContext _context;

    public LearningComponentRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId)
    {
        return await _context.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId)
            .ToListAsync();
    }
}
