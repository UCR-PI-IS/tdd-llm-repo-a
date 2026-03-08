using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers
{
    [TestFixture]
    public class CreateWhiteboardHandlerTests
    {
        private Mock<IWhiteboardService> _mockWhiteboardService;
        private CreateWhiteboardHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockWhiteboardService = new Mock<IWhiteboardService>();
            _handler = new CreateWhiteboardHandler(_mockWhiteboardService.Object);
        }

        [Test]
        [TestCase("WB-001", 2.0, 1.5, "LS-001", Description = "Returns 201 Created when whiteboard creation succeeds")]
        [TestCase("WB-002", 5.0, 5.0, "LS-002", Description = "Returns 201 Created with large dimensions")]
        [TestCase("WB-003", 0.5, 0.5, "LS-003", Description = "Returns 201 Created with small dimensions")]
        public async Task HandleAsync_WhenServiceSucceeds_Returns201Created(
            string whiteboardId,
            double width,
            double height,
            string learningSpaceId)
        {
            // Arrange
            var successResult = Result.Success();
            _mockWhiteboardService
                .Setup(s => s.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId))
                .ReturnsAsync(successResult);
            
            var request = new CreateWhiteboardRequest(whiteboardId, width, height, learningSpaceId);

            // Act
            var result = await _handler.HandleAsync(request);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Response.Success, Is.True);
            _mockWhiteboardService.Verify(s => s.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId), Times.Once);
        }

        [Test]
        [TestCase("WB-001", 10.0, 10.0, "LS-001", "Whiteboard doesn't fit in learning space", Description = "Returns 400 when whiteboard doesn't fit")]
        [TestCase("WB-002", 2.0, 1.5, "LS-999", "Learning space not found", Description = "Returns 400 when learning space not found")]
        [TestCase("WB-003", -1.0, 1.5, "LS-001", "Width must be positive", Description = "Returns 400 when width is invalid")]
        [TestCase("WB-004", 2.0, -1.5, "LS-001", "Height must be positive", Description = "Returns 400 when height is invalid")]
        public async Task HandleAsync_WhenServiceFails_Returns400BadRequest(
            string whiteboardId,
            double width,
            double height,
            string learningSpaceId,
            string errorMessage)
        {
            // Arrange
            var failureResult = Result.Failure(errorMessage);
            _mockWhiteboardService
                .Setup(s => s.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId))
                .ReturnsAsync(failureResult);
            
            var request = new CreateWhiteboardRequest(whiteboardId, width, height, learningSpaceId);

            // Act
            var result = await _handler.HandleAsync(request);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Response.Success, Is.False);
            Assert.That(result.Response.ErrorMessage, Is.EqualTo(errorMessage));
            _mockWhiteboardService.Verify(s => s.CreateWhiteboardAsync(whiteboardId, width, height, learningSpaceId), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            _mockWhiteboardService = null;
            _handler = null;
        }
    }
}
