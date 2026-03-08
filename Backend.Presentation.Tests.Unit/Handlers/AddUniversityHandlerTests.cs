using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers
{
    [TestFixture]
    public class AddUniversityHandlerTests
    {
        private Mock<IUniversityService> _mockService;
        private AddUniversityHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IUniversityService>();
            _handler = new AddUniversityHandler(_mockService.Object);
        }

        [Test]
        [Description("Handle returns success response when university is added successfully")]
        public async Task Handle_SuccessfulAdd_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new AddUniversityRequest { Name = "UCR", Country = "Costa Rica" };
            _mockService.Setup(s => s.AddUniversityAsync("UCR", "Costa Rica")).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Does.Contain("successfully"));
            _mockService.Verify(s => s.AddUniversityAsync("UCR", "Costa Rica"), Times.Once);
        }

        [Test]
        [Description("Handle returns failure response when service returns false")]
        public async Task Handle_ServiceReturnsFalse_ReturnsFailureResponse()
        {
            // Arrange
            var request = new AddUniversityRequest { Name = "UCR", Country = "Costa Rica" };
            _mockService.Setup(s => s.AddUniversityAsync("UCR", "Costa Rica")).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("failed"));
        }

        [Test]
        [Description("Handle returns error response when service throws exception")]
        public async Task Handle_ServiceThrowsException_ReturnsErrorResponse()
        {
            // Arrange
            var request = new AddUniversityRequest { Name = "UCR", Country = "Costa Rica" };
            _mockService.Setup(s => s.AddUniversityAsync("UCR", "Costa Rica"))
                .ThrowsAsync(new InvalidOperationException("University with name 'UCR' already exists"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("already exists"));
        }
    }
}
