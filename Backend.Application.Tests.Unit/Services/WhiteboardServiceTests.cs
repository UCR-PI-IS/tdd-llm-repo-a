using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services
{
    [TestFixture]
    public class WhiteboardServiceTests
    {
        private Mock<IWhiteboardRepository> _mockWhiteboardRepository;
        private Mock<ILearningSpaceRepository> _mockLearningSpaceRepository;
        private IWhiteboardService _whiteboardService;

        [SetUp]
        public void SetUp()
        {
            _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
            _mockLearningSpaceRepository = new Mock<ILearningSpaceRepository>();
            _whiteboardService = new WhiteboardService(_mockWhiteboardRepository.Object, _mockLearningSpaceRepository.Object);
        }

        [Test]
        [TestCase("WB-001", 2.0, 1.5, "LS-001", 5.0, 5.0, Description = "Successfully creates whiteboard when it fits in learning space")]
        [TestCase("WB-002", 5.0, 5.0, "LS-002", 5.0, 5.0, Description = "Successfully creates whiteboard when dimensions equal learning space")]
        [TestCase("WB-003", 0.5, 0.5, "LS-003", 10.0, 10.0, Description = "Successfully creates whiteboard with small dimensions in large space")]
        public async Task CreateWhiteboardAsync_WithValidParameters_CreatesWhiteboardSuccessfully(
            string whiteboardId, 
            double width, 
            double height, 
            string learningSpaceId, 
            double spaceWidth, 
            double spaceHeight)
        {
            // Arrange
            var learningSpace = new LearningSpace(learningSpaceId, "Test Space", spaceWidth, spaceHeight);
            _mockLearningSpaceRepository
                .Setup(r => r.GetByIdAsync(learningSpaceId))
                .ReturnsAsync(learningSpace);
            
            _mockWhiteboardRepository
                .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _whiteboardService.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            _mockLearningSpaceRepository.Verify(r => r.GetByIdAsync(learningSpaceId), Times.Once);
            _mockWhiteboardRepository.Verify(r => r.AddAsync(It.Is<Whiteboard>(w => 
                w.Id == whiteboardId && 
                w.Width == width && 
                w.Height == height)), Times.Once);
        }

        [Test]
        [TestCase("WB-001", 6.0, 1.5, "LS-001", 5.0, 5.0, Description = "Fails when whiteboard width exceeds learning space")]
        [TestCase("WB-002", 2.0, 6.0, "LS-002", 5.0, 5.0, Description = "Fails when whiteboard height exceeds learning space")]
        [TestCase("WB-003", 10.0, 10.0, "LS-003", 5.0, 5.0, Description = "Fails when both whiteboard dimensions exceed learning space")]
        public async Task CreateWhiteboardAsync_WhenWhiteboardDoesNotFit_ReturnsFailure(
            string whiteboardId, 
            double width, 
            double height, 
            string learningSpaceId, 
            double spaceWidth, 
            double spaceHeight)
        {
            // Arrange
            var learningSpace = new LearningSpace(learningSpaceId, "Test Space", spaceWidth, spaceHeight);
            _mockLearningSpaceRepository
                .Setup(r => r.GetByIdAsync(learningSpaceId))
                .ReturnsAsync(learningSpace);

            // Act
            var result = await _whiteboardService.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("fit").IgnoreCase);
            _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
        }

        [Test]
        [TestCase("WB-001", 2.0, 1.5, "LS-999", Description = "Fails when learning space does not exist")]
        [TestCase("WB-002", 3.0, 2.0, "LS-INVALID", Description = "Fails when learning space ID is invalid")]
        public async Task CreateWhiteboardAsync_WhenLearningSpaceNotFound_ReturnsFailure(
            string whiteboardId, 
            double width, 
            double height, 
            string learningSpaceId)
        {
            // Arrange
            _mockLearningSpaceRepository
                .Setup(r => r.GetByIdAsync(learningSpaceId))
                .ReturnsAsync((LearningSpace)null);

            // Act
            var result = await _whiteboardService.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("not found").IgnoreCase);
            _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
        }

        [Test]
        [TestCase("WB-001", -2.0, 1.5, "LS-001", Description = "Fails when whiteboard width is negative")]
        [TestCase("WB-002", 2.0, -1.5, "LS-001", Description = "Fails when whiteboard height is negative")]
        [TestCase("WB-003", 0.0, 1.5, "LS-001", Description = "Fails when whiteboard width is zero")]
        [TestCase("WB-004", 2.0, 0.0, "LS-001", Description = "Fails when whiteboard height is zero")]
        public async Task CreateWhiteboardAsync_WhenDomainValidationFails_ReturnsFailure(
            string whiteboardId, 
            double width, 
            double height, 
            string learningSpaceId)
        {
            // Arrange
            var learningSpace = new LearningSpace(learningSpaceId, "Test Space", 5.0, 5.0);
            _mockLearningSpaceRepository
                .Setup(r => r.GetByIdAsync(learningSpaceId))
                .ReturnsAsync(learningSpace);

            // Act
            var result = await _whiteboardService.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
            _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
        }

        [TearDown]
        public void TearDown()
        {
            _mockWhiteboardRepository = null;
            _mockLearningSpaceRepository = null;
            _whiteboardService = null;
        }
    }
}
