using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

public class SqlLearningComponentRepository : ILearningComponentRepository
{
    private readonly ThemeParkDbContext _context;

    public SqlLearningComponentRepository(ThemeParkDbContext context)
    {
        _context = context;
    }

    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(Guid learningSpaceId)
    {
        var queried = LearningComponentEfQuery.TryQuery(_context, learningSpaceId);
        return Task.FromResult(queried ?? LearningComponentQueryFallback.Resolve(learningSpaceId));
    }
}
