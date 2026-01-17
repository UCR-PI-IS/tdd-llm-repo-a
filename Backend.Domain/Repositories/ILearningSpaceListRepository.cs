using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories
{
    /// <summary>
    /// Contract for accessing learning space data sources.
    /// </summary>
    public interface ILearningSpaceListRepository
    {
        List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId);
        System.Threading.Tasks.Task<LearningSpace> GetCurrentLearningSpaceListAsync();
        System.Threading.Tasks.Task<List<LearningSpace>> GetAllLearningSpacesAsync();
    }
}
