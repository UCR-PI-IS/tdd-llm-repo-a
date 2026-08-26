using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningSpaceCreateService"/> class.
/// Covers Application-001 through Application-005 intents.
/// </summary>
[TestFixture]
public class LearningSpaceCreateServiceTests
{
    private Mock<ILearningSpaceRepository> _mockRepository = null!;
    private LearningSpaceCreateService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningSpaceRepository>();
        _service = new LearningSpaceCreateService(_mockRepository.Object);
    }

    /// <summary>
    /// Application-001: Verify that the service successfully creates a learning space with valid input
    /// and returns the created entity with generated ID.
    /// </summary>
    [Test]
    [Description("Application-001: Verify that the service successfully creates a learning space with valid input and returns the created entity with generated ID")]
    public async Task CreateLearningSpaceAsync_ValidInput_ReturnsCreatedEntity()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act
        var result = await _service.CreateLearningSpaceAsync(type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Type, Is.EqualTo(type));
            Assert.That(result.Height, Is.EqualTo(height));
            Assert.That(result.Width, Is.EqualTo(width));
            Assert.That(result.Length, Is.EqualTo(length));
            Assert.That(result.LearningSpaceId, Is.GreaterThan(0));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that the service propagates ArgumentException when domain validation fails for invalid type.
    /// </summary>
    [Test]
    [Description("Application-002: Verify that the service propagates ArgumentException when domain validation fails for invalid type")]
    public void CreateLearningSpaceAsync_InvalidType_PropagatesArgumentException()
    {
        // Arrange
        var invalidType = "InvalidType";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateLearningSpaceAsync(invalidType, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that the service propagates ArgumentException when domain validation fails for zero dimension.
    /// </summary>
    [Test]
    [Description("Application-003: Verify that the service propagates ArgumentException when domain validation fails for zero dimension")]
    public void CreateLearningSpaceAsync_ZeroHeight_PropagatesArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 0.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateLearningSpaceAsync(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that the service propagates ArgumentException when domain validation fails for negative dimension.
    /// </summary>
    [Test]
    [Description("Application-004: Verify that the service propagates ArgumentException when domain validation fails for negative dimension")]
    public void CreateLearningSpaceAsync_NegativeWidth_PropagatesArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = -8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateLearningSpaceAsync(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service propagates repository exceptions when database operation fails.
    /// </summary>
    [Test]
    [Description("Application-005: Verify that the service propagates repository exceptions when database operation fails")]
    public void CreateLearningSpaceAsync_RepositoryException_PropagatesException()
    {
        // Arrange
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateLearningSpaceAsync(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Database connection failed"));
    }
}
