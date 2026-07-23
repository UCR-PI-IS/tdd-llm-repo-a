using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit
{
    [TestFixture]
    public class SqlLearningComponentRepositoryTests
    {
        private Mock<ThemeParkDbContext> _mockContext = null!;
        private SqlLearningComponentRepository _repository = null!;
        private Guid _learningSpaceId;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ThemeParkDbContext>();
            _repository = new SqlLearningComponentRepository(_mockContext.Object);
            _learningSpaceId = Guid.NewGuid();
        }

        [Test]
        [Description("Verify repository returns list of components for a valid learning space ID from database")]
        public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithComponents_ReturnsList()
        {
            // Arrange
            // Repository is expected to query the context for components matching the learning space ID.

            // Act
            var result = await _repository.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.All(c => c.LearningSpaceId == _learningSpaceId), Is.True);
            });
        }

        [Test]
        [Description("Verify repository returns empty list when learning space has no components")]
        public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
        {
            // Arrange
            // Learning space exists but has no associated components in the data store.

            // Act
            var result = await _repository.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(0));
                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        [Description("Verify repository returns empty list when learning space ID does not exist in database")]
        public async Task GetComponentsByLearningSpaceIdAsync_NonExistentId_ReturnsEmptyList()
        {
            // Arrange
            var nonExistentLearningSpaceId = Guid.NewGuid();

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
