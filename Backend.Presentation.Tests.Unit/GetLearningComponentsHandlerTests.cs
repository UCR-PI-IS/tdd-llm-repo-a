using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit
{
    /// <summary>
    /// Unit tests for the GetLearningComponentsHandler.
    /// </summary>
    [TestFixture]
    public class GetLearningComponentsHandlerTests
    {
        private Mock<ILearningComponentService> _mockService = null!;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ILearningComponentService>();
        }

        /// <summary>
        /// Test that handler returns OK response with list of components when learning space has components.
        /// </summary>
        [Test]
        [Description("Verify handler returns OK response with list of components when learning space has components")]
        public async Task HandleAsync_HasComponents_ReturnsOkWithComponents()
        {
            // Arrange
            var learningSpaceId = "space-001";
            var components = new List<LearningComponent>
            {
                new LearningComponent("comp-001", learningSpaceId, 2.5f, 3.0f, 2.0f, 1.0f, 0.5f, 2.0f, "North"),
                new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.0f, 1.5f, 3.0f, 0.5f, 4.0f, "South")
            };

            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
                .ReturnsAsync(components);

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

            // Assert
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.Multiple(() =>
            {
                Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(2));
                Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
            });
        }

        /// <summary>
        /// Test that handler returns OK response with empty list when learning space has no components.
        /// </summary>
        [Test]
        [Description("Verify handler returns OK response with empty list when learning space has no components")]
        public async Task HandleAsync_NoComponents_ReturnsOkWithEmptyList()
        {
            // Arrange
            var learningSpaceId = "space-001";
            var emptyComponents = new List<LearningComponent>();

            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
                .ReturnsAsync(emptyComponents);

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

            // Assert
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.Multiple(() =>
            {
                Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(0));
                Assert.That(okResult.Value.Components, Is.Empty);
            });
        }

        /// <summary>
        /// Test that handler returns BadRequest response when learning space ID is null or empty.
        /// </summary>
        [Test]
        [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
        public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest()
        {
            // Arrange
            var invalidLearningSpaceId = string.Empty;

            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId))
                .ThrowsAsync(new ArgumentException("Learning space ID cannot be null or empty", "learningSpaceId"));

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
            var badRequestResult = result as BadRequest<ErrorResponse>;
            Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
        }

        /// <summary>
        /// Test that handler returns NotFound response when learning space does not exist.
        /// </summary>
        [Test]
        [Description("Verify handler returns NotFound response when learning space does not exist")]
        public async Task HandleAsync_NonExistentSpace_ReturnsNotFound()
        {
            // Arrange
            var nonExistentLearningSpaceId = "non-existent-space";

            _mockService
                .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
                .ThrowsAsync(new KeyNotFoundException($"Learning space with ID '{nonExistentLearningSpaceId}' not found"));

            // Act
            var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
            var notFoundResult = result as NotFound<ErrorResponse>;
            Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
        }
    }
}
