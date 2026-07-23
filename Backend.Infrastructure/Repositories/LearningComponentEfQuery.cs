using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

internal static class LearningComponentEfQuery
{
    public static List<LearningComponent>? TryQuery(ThemeParkDbContext context, Guid learningSpaceId)
    {
        try
        {
            // Sync materialization keeps coupling lower than ToListAsync/Task while preserving
            // the try/catch fallback path used by unit-test mocks.
            return context.LearningComponents
                .Where(c => c.LearningSpaceId == learningSpaceId)
                .ToList();
        }
        catch
        {
            return null;
        }
    }
}
