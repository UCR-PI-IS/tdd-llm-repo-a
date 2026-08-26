using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the LearningSpaceCreateService.
/// </summary>
[TestFixture]
public class LearningSpaceCreateServiceTests
{
    private Mock<ILearningSpaceRepository> _mockRepository = null!;
    private LearningSpaceCreateService _service = null!;

    /// <summary>
    /// Sets up the test fixture before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<ILearningSpaceRepository>();
        _service = new LearningSpaceCreateService(_mockRepository.Object);
    }

    /// <summary>
    /// Verifies that the service successfully creates a learning space with valid input.
    /// </summary>
    [Test]
    [Description("Creates a learning space with valid parameters and verifies repository interaction")]
    public async Task CreateLearningSpaceAsync_WithValidParameters_ReturnsCreatedSpace()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        var expectedSpace = new LearningSpace(id, type, height, width, length);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateLearningSpaceAsync(id, type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.id, Is.EqualTo(id));
            Assert.That(result.type, Is.EqualTo(type));
            Assert.That(result.height, Is.EqualTo(height));
            Assert.That(result.width, Is.EqualTo(width));
            Assert.That(result.length, Is.EqualTo(length));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Once);
    }

    /// <summary>
    /// Verifies that the service propagates ArgumentException when domain validation fails for invalid type.
    /// </summary>
    [Test]
    [Description("Validates that invalid type propagates ArgumentException and does not call repository")]
    public void CreateLearningSpaceAsync_WithInvalidType_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var invalidType = "InvalidType";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateLearningSpaceAsync(id, invalidType, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the service propagates ArgumentException when domain validation fails for zero height.
    /// </summary>
    [Test]
    [Description("Validates that zero height propagates ArgumentException and does not call repository")]
    public void CreateLearningSpaceAsync_WithZeroHeight_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 0.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateLearningSpaceAsync(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the service propagates ArgumentException when domain validation fails for negative width.
    /// </summary>
    [Test]
    [Description("Validates that negative width propagates ArgumentException and does not call repository")]
    public void CreateLearningSpaceAsync_WithNegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = -8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateLearningSpaceAsync(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the service propagates repository exceptions when database operation fails.
    /// </summary>
    [Test]
    [Description("Validates that repository exceptions are propagated to caller")]
    public void CreateLearningSpaceAsync_WhenRepositoryThrows_PropagatesException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateLearningSpaceAsync(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Database connection failed"));
    }
}
