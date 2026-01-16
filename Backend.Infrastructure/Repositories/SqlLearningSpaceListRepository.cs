using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories
{
    /// <summary>
    /// SQL-based implementation of <see cref="ILearningSpaceListRepository"/>.
    /// Provides access to learning space data stored in the database.
    /// </summary>
    internal class SqlLearningSpaceListRepository : ILearningSpaceListRepository
    {
        private readonly UCRDatabaseContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLearningSpaceListRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context used for data access.</param>
        public SqlLearningSpaceListRepository(UCRDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves a predefined learning space with ID "IF-0103".
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the matching <see cref="LearningSpace"/>.</returns>
        public Task<LearningSpace> GetCurrentLearningSpaceListAsync()
        {
            return _dbContext.LearningSpaces
                .FirstAsync(LearningSpaces => LearningSpaces.id == "IF-0103");
        }

        /// <summary>
        /// Retrieves all learning spaces from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="LearningSpace"/> entities.</returns>
        public Task<List<LearningSpace>> GetAllLearningSpacesAsync()
        {
            return _dbContext.LearningSpaces.ToListAsync();
        }

        /// <summary>
        /// Gets the components by learning space identifier.
        /// </summary>
        /// <param name="learningSpaceId">The learning space identifier.</param>
        /// <returns>List of components.</returns>
        public List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId)
        {
            var learningSpace = _dbContext.LearningSpaces
                .Include(ls => ls.Components)
                .FirstOrDefault(ls => ls.Id == learningSpaceId.ToString());
            if (learningSpace == null)
            {
                throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
            }

            return learningSpace.Components.ToList();
        }
    }
}
