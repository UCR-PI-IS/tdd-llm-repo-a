using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers
{
    [TestFixture]
    public class AddBuildingHandlerTests
    {
        private Mock<IBuildingService> _mockService;
        private AddBuildingHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IBuildingService>();
            _handler = new AddBuildingHandler(_mockService.Object);
        }

        [Test(Description = "Successfully adds building and returns success response")]
        public async Task HandleAsync_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new AddBuildingRequest
            {
                Name = "Engineering Building",
                Color = "Blue",
                Height = 15.5,
                Length = 30.0,
                Width = 20.0,
                X = 100.0,
                Y = 200.0,
                Z = 0.0
            };

            _mockService.Setup(s => s.AddBuildingAsync(
                "Engineering Building", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.HandleAsync(request);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null.Or.Empty);
            _mockService.Verify(s => s.AddBuildingAsync(
                "Engineering Building", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0), Times.Once);
        }

        [Test(Description = "Returns failure response when InvalidOperationException is thrown")]
        public async Task HandleAsync_WhenDuplicateBuilding_ReturnsFailureResponse()
        {
            // Arrange
            var request = new AddBuildingRequest
            {
                Name = "Engineering Building",
                Color = "Blue",
                Height = 15.5,
                Length = 30.0,
                Width = 20.0,
                X = 100.0,
                Y = 200.0,
                Z = 0.0
            };

            _mockService.Setup(s => s.AddBuildingAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), 
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), 
                It.IsAny<double>(), It.IsAny<double>()))
                .ThrowsAsync(new InvalidOperationException("Building with this name already exists"));

            // Act
            var result = await _handler.HandleAsync(request);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
        }

        [Test(Description = "Returns failure response when ArgumentException is thrown")]
        public async Task HandleAsync_WithInvalidInput_ReturnsFailureResponse()
        {
            // Arrange
            var request = new AddBuildingRequest
            {
                Name = null,
                Color = "Blue",
                Height = 15.5,
                Length = 30.0,
                Width = 20.0,
                X = 100.0,
                Y = 200.0,
                Z = 0.0
            };

            _mockService.Setup(s => s.AddBuildingAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), 
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), 
                It.IsAny<double>(), It.IsAny<double>()))
                .ThrowsAsync(new ArgumentException("Name is required"));

            // Act
            var result = await _handler.HandleAsync(request);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name is required"));
        }
    }
}
