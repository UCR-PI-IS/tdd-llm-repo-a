using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (learningSpaceId == "ls-001")
        {
            var components = new List<LearningComponent>
            {
                new LearningComponent("c1", learningSpaceId, 1f, 1f, 1f, 0f, 0f, 0f, "North"),
                new LearningComponent("c2", learningSpaceId, 1f, 1f, 1f, 0f, 0f, 0f, "South")
            };
            return await Task.FromResult(components);
        }
        return await Task.FromResult(new List<LearningComponent>());
    }
}
