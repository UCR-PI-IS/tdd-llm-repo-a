using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit
{
    [TestFixture]
    public class GetLearningComponentsHandlerTests
    {
        private Mock<ILearningComponentService> _mockService = null!;
        private Guid _learningSpaceId;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ILearningComponentService>();
            _learningSpaceId = Guid.NewGuid();
        }

        [Test]
        [Description("Verify handler returns OK response with list of components when learning space has components")]
        public async Task HandleAsync_ExistingComponents_ReturnsOkWithComponents()
        {
            // Arrange
            var components = new List<LearningComponent>
            {
                new LearningComponent(Guid.NewGuid(), _learningSpaceId, 10f, 5f, 2f, 0f, 0f, 0f, "North"),
                new LearningComponent(Guid.NewGuid(), _learningSpaceId, 10f, 5f, 2f, 1f, 1f, 0f, "South")
            };
            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_learningSpaceId.ToString()))
                .ReturnsAsync(components);

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(
                _mockService.Object,
                _learningSpaceId.ToString());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
                var okResult = result as Ok<GetLearningComponentsResponse>;
                Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(2));
                Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(_learningSpaceId));
            });
        }

        [Test]
        [Description("Verify handler returns OK response with empty list when learning space has no components")]
        public async Task HandleAsync_NoComponents_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_learningSpaceId.ToString()))
                .ReturnsAsync(new List<LearningComponent>());

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(
                _mockService.Object,
                _learningSpaceId.ToString());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
                var okResult = result as Ok<GetLearningComponentsResponse>;
                Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(0));
                Assert.That(okResult.Value.Components, Is.Empty);
            });
        }

        [Test]
        [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
        public async Task HandleAsync_EmptyLearningSpaceId_ReturnsBadRequest()
        {
            // Arrange
            var invalidLearningSpaceId = string.Empty;

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(
                _mockService.Object,
                invalidLearningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
                var badRequestResult = result as BadRequest<ErrorResponse>;
                Assert.That(
                    badRequestResult!.Value!.Message,
                    Does.Contain("Learning space ID cannot be null or empty"));
            });
        }

        [Test]
        [Description("Verify handler returns NotFound response when learning space does not exist")]
        public async Task HandleAsync_NonExistentLearningSpace_ReturnsNotFound()
        {
            // Arrange
            var nonExistentLearningSpaceId = Guid.NewGuid().ToString();
            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
                .ThrowsAsync(new InvalidOperationException("Learning space not found"));

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(
                _mockService.Object,
                nonExistentLearningSpaceId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
                var notFoundResult = result as NotFound<ErrorResponse>;
                Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
            });
        }
    }
}
