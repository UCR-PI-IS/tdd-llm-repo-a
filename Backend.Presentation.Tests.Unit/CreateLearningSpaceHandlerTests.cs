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
    [Description("Presentation-001: Handler returns 201 Created with the created learning space resource")]
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
        Assert.That(result.Result, Is.InstanceOf<Created<LearningSpaceResponse>>());
    }

    /// <summary>
    /// Presentation-002, Presentation-003, Presentation-004: Verify that the handler returns
    /// 400 Bad Request when the service throws ArgumentException for invalid type, zero dimension,
    /// or negative dimension.
    /// </summary>
    [TestCase("InvalidType", 3.0f, 8.0f, 10.0f, "Type must be Classroom, Auditorium, or Laboratory", "Type must be Classroom",
        Description = "Presentation-002: Invalid type returns BadRequest")]
    [TestCase("Classroom", 0.0f, 8.0f, 10.0f, "Height must be positive and non-zero", "Height must be positive",
        Description = "Presentation-003: Zero dimension returns BadRequest")]
    [TestCase("Classroom", 3.0f, -8.0f, 10.0f, "Width must be positive and non-zero", "Width must be positive",
        Description = "Presentation-004: Negative dimension returns BadRequest")]
    public async Task HandleAsync_ArgumentException_ReturnsBadRequest(
        string type, float height, float width, float length,
        string exceptionMessage, string expectedBodySubstring)
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateLearningSpaceAsync(type, height, width, length))
            .ThrowsAsync(new ArgumentException(exceptionMessage));

        var dto = new CreateLearningSpaceDto(type, height, width, length);

        // Act
        var result = await CreateLearningSpaceHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain(expectedBodySubstring));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that the handler returns 500 Internal Server Error
    /// when the service throws an unexpected exception.
    /// </summary>
    [Test]
    [Description("Presentation-005: Handler returns 500 Internal Server Error for unexpected exceptions")]
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
