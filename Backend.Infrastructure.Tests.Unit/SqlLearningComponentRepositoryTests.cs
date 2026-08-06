using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Data;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit
{
    /// <summary>
    /// Unit tests for the SqlLearningComponentRepository.
    /// </summary>
    [TestFixture]
    public class SqlLearningComponentRepositoryTests
    {
        private AppDbContext _dbContext = null!;
        private SqlLearningComponentRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;

            _dbContext = new AppDbContext(options);
            _repository = new SqlLearningComponentRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        /// <summary>
        /// Test that repository returns list of components for a valid learning space ID from database.
        /// </summary>
        [Test]
        [Description("Verify repository returns list of components for a valid learning space ID from database")]
        public async Task GetComponentsByLearningSpaceIdAsync_HasComponents_ReturnsComponentList()
        {
            // Arrange
            var learningSpaceId = "space-001";
            var components = new List<LearningComponent>
            {
                new LearningComponent("comp-001", learningSpaceId, 2.5f, 3.0f, 2.0f, 1.0f, 0.5f, 2.0f, "North"),
                new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.0f, 1.5f, 3.0f, 0.5f, 4.0f, "South")
            };

            _dbContext.LearningComponents.AddRange(components);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
            });
        }

        /// <summary>
        /// Test that repository returns empty list when learning space has no components.
        /// </summary>
        [Test]
        [Description("Verify repository returns empty list when learning space has no components")]
        public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
        {
            // Arrange
            var learningSpaceId = "space-001";
            // No components added for this learning space

            // Act
            var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(0));
                Assert.That(result, Is.Empty);
            });
        }

        /// <summary>
        /// Test that repository returns empty list when learning space ID does not exist in database.
        /// </summary>
        [Test]
        [Description("Verify repository returns empty list when learning space ID does not exist in database")]
        public async Task GetComponentsByLearningSpaceIdAsync_NonExistentSpace_ReturnsEmptyList()
        {
            // Arrange
            var nonExistentLearningSpaceId = "non-existent-space";
            // Add components for other learning spaces
            _dbContext.LearningComponents.Add(new LearningComponent("comp-001", "other-space", 2.5f, 3.0f, 2.0f, 1.0f, 0.5f, 2.0f, "North"));
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(0));
                Assert.That(result, Is.Empty);
            });
        }
    }
}
