using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the CreateLearningSpaceHandler.
/// </summary>
[TestFixture]
public class CreateLearningSpaceHandlerTests
{
    private Mock<ILearningSpaceCreateService> _mockService = null!;

    /// <summary>
    /// Sets up the test fixture before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<ILearningSpaceCreateService>();
    }

    /// <summary>
    /// Verifies that the handler successfully creates a learning space and returns 201 Created.
    /// </summary>
    [Test]
    [Description("Creates a learning space and returns 201 Created with the resource")]
    public async Task HandleAsync_WithValidDto_ReturnsCreatedResult()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        var createdSpace = new LearningSpace(id, type, height, width, length);
        _mockService.Setup(s => s.CreateLearningSpaceAsync(id, type, height, width, length))
            .ReturnsAsync(createdSpace);

        var dto = new CreateLearningSpaceDto(id, type, height, width, length);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Created<LearningSpaceResponse>>());
        var createdResult = (Created<LearningSpaceResponse>)result.Result;
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.Value.Id, Is.EqualTo(id));
            Assert.That(createdResult.Value.Type, Is.EqualTo(type));
            Assert.That(createdResult.Value.Height, Is.EqualTo(height));
            Assert.That(createdResult.Value.Width, Is.EqualTo(width));
            Assert.That(createdResult.Value.Length, Is.EqualTo(length));
        });
    }

    /// <summary>
    /// Verifies that the handler returns 400 Bad Request when service throws ArgumentException for invalid type.
    /// </summary>
    [Test]
    [Description("Returns 400 Bad Request when service throws ArgumentException for invalid type")]
    public async Task HandleAsync_WithInvalidType_ReturnsBadRequest()
    {
        // Arrange
        var id = "IF-0101";
        var invalidType = "InvalidType";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        _mockService.Setup(s => s.CreateLearningSpaceAsync(id, invalidType, height, width, length))
            .ThrowsAsync(new ArgumentException("Type must be Classroom, Auditorium, or Laboratory"));

        var dto = new CreateLearningSpaceDto(id, invalidType, height, width, length);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
        var badRequestResult = (BadRequest<string>)result.Result;
        Assert.That(badRequestResult.Value, Does.Contain("Type must be Classroom"));
    }

    /// <summary>
    /// Verifies that the handler returns 400 Bad Request when service throws ArgumentException for zero dimension.
    /// </summary>
    [Test]
    [Description("Returns 400 Bad Request when service throws ArgumentException for zero height")]
    public async Task HandleAsync_WithZeroHeight_ReturnsBadRequest()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 0.0f;
        var width = 8.0f;
        var length = 10.0f;

        _mockService.Setup(s => s.CreateLearningSpaceAsync(id, type, height, width, length))
            .ThrowsAsync(new ArgumentException("Height must be positive and non-zero"));

        var dto = new CreateLearningSpaceDto(id, type, height, width, length);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
        var badRequestResult = (BadRequest<string>)result.Result;
        Assert.That(badRequestResult.Value, Does.Contain("Height must be positive"));
    }

    /// <summary>
    /// Verifies that the handler returns 400 Bad Request when service throws ArgumentException for negative dimension.
    /// </summary>
    [Test]
    [Description("Returns 400 Bad Request when service throws ArgumentException for negative width")]
    public async Task HandleAsync_WithNegativeWidth_ReturnsBadRequest()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = -8.0f;
        var length = 10.0f;

        _mockService.Setup(s => s.CreateLearningSpaceAsync(id, type, height, width, length))
            .ThrowsAsync(new ArgumentException("Width must be positive and non-zero"));

        var dto = new CreateLearningSpaceDto(id, type, height, width, length);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
        var badRequestResult = (BadRequest<string>)result.Result;
        Assert.That(badRequestResult.Value, Does.Contain("Width must be positive"));
    }

    /// <summary>
    /// Verifies that the handler returns 500 Internal Server Error when service throws unexpected exception.
    /// </summary>
    [Test]
    [Description("Returns 500 Internal Server Error when service throws unexpected exception")]
    public async Task HandleAsync_WhenServiceThrowsUnexpectedException_ReturnsProblemDetails()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        _mockService.Setup(s => s.CreateLearningSpaceAsync(id, type, height, width, length))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        var dto = new CreateLearningSpaceDto(id, type, height, width, length);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ProblemHttpResult>());
    }
}
