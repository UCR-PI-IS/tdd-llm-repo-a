using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit
{
    [TestFixture]
    public class LearningComponentServiceTests
    {
        private Mock<ILearningComponentRepository> _mockRepository = null!;
        private LearningComponentService _service = null!;
        private Guid _learningSpaceId;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ILearningComponentRepository>();
            _service = new LearningComponentService(_mockRepository.Object);
            _learningSpaceId = Guid.NewGuid();
        }

        [Test]
        [Description("Verify service returns list of components when learning space has components")]
        public async Task GetComponentsByLearningSpaceIdAsync_ExistingComponents_ReturnsList()
        {
            // Arrange
            var components = new List<LearningComponent>
            {
                new LearningComponent(Guid.NewGuid(), _learningSpaceId, 10f, 5f, 2f, 0f, 0f, 0f, "North"),
                new LearningComponent(Guid.NewGuid(), _learningSpaceId, 10f, 5f, 2f, 1f, 1f, 0f, "South")
            };
            _mockRepository
                .Setup(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
                .ReturnsAsync(components);

            // Act
            var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].LearningSpaceId, Is.EqualTo(_learningSpaceId));
                Assert.That(result[1].LearningSpaceId, Is.EqualTo(_learningSpaceId));
            });
        }

        [Test]
        [Description("Verify service returns empty list when learning space has no components")]
        public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
                .ReturnsAsync(new List<LearningComponent>());

            // Act
            var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(0));
                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        [Description("Verify service throws exception when learning space ID is null or empty")]
        public void GetComponentsByLearningSpaceIdAsync_EmptyId_ThrowsArgumentException()
        {
            // Arrange
            var invalidLearningSpaceId = string.Empty;

            // Act & Assert
            Assert.That(
                async () => await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("learningSpaceId"));
        }

        [Test]
        [Description("Verify service throws exception when learning space ID is null")]
        public void GetComponentsByLearningSpaceIdAsync_NullId_ThrowsArgumentException()
        {
            // Arrange
            string? nullLearningSpaceId = null;

            // Act & Assert
            Assert.That(
                async () => await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("learningSpaceId"));
        }
    }
}
