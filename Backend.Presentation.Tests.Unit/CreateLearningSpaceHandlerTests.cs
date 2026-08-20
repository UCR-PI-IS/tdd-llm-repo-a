using Microsoft.AspNetCore.Http;
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
/// Unit tests for <see cref="CreateLearningSpaceHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005.
/// </summary>
[TestFixture]
public class CreateLearningSpaceHandlerTests
{
    private Mock<ILearningSpaceCreateService> _mockService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningSpaceCreateService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-001: Verify that the handler successfully creates a learning space
    /// and returns 201 Created with the created resource.
    /// </summary>
    [Test]
    [Description("Presentation-001: Verify handler returns 201 Created with the created resource")]
    public async Task HandleAsync_ValidInput_ReturnsCreated()
    {
        // Arrange
        var createdSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);
        _mockService
            .Setup(s => s.CreateLearningSpaceAsync("Classroom", 3.0f, 8.0f, 10.0f))
            .ReturnsAsync(createdSpace);
        var dto = new CreateLearningSpaceDto("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Created<LearningSpaceResponse>>());
            var createdResult = (Created<LearningSpaceResponse>)result.Result;
            Assert.That(createdResult.Value!.LearningSpaceId, Is.GreaterThan(0));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that the handler returns 400 Bad Request when service
    /// throws ArgumentException for invalid type.
    /// </summary>
    [Test]
    [Description("Presentation-002: Verify handler returns 400 Bad Request for invalid type")]
    public async Task HandleAsync_InvalidType_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateLearningSpaceAsync("InvalidType", 3.0f, 8.0f, 10.0f))
            .ThrowsAsync(new ArgumentException("Type must be Classroom, Auditorium, or Laboratory"));
        var dto = new CreateLearningSpaceDto("InvalidType", 3.0f, 8.0f, 10.0f);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Type must be Classroom"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that the handler returns 400 Bad Request when service
    /// throws ArgumentException for zero dimension.
    /// </summary>
    [Test]
    [Description("Presentation-003: Verify handler returns 400 Bad Request for zero dimension")]
    public async Task HandleAsync_ZeroDimension_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateLearningSpaceAsync("Classroom", 0.0f, 8.0f, 10.0f))
            .ThrowsAsync(new ArgumentException("Height must be positive and non-zero"));
        var dto = new CreateLearningSpaceDto("Classroom", 0.0f, 8.0f, 10.0f);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Height must be positive"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify that the handler returns 400 Bad Request when service
    /// throws ArgumentException for negative dimension.
    /// </summary>
    [Test]
    [Description("Presentation-004: Verify handler returns 400 Bad Request for negative dimension")]
    public async Task HandleAsync_NegativeDimension_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateLearningSpaceAsync("Classroom", 3.0f, -8.0f, 10.0f))
            .ThrowsAsync(new ArgumentException("Width must be positive and non-zero"));
        var dto = new CreateLearningSpaceDto("Classroom", 3.0f, -8.0f, 10.0f);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Width must be positive"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that the handler returns 500 Internal Server Error
    /// when service throws an unexpected exception.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify handler returns 500 Internal Server Error for unexpected exception")]
    public async Task HandleAsync_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateLearningSpaceAsync("Classroom", 3.0f, 8.0f, 10.0f))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));
        var dto = new CreateLearningSpaceDto("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ProblemHttpResult>());
    }
}
