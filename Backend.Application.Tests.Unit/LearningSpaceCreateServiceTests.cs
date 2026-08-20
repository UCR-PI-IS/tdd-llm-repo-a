using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningSpaceCreateService.CreateLearningSpaceAsync"/>.
/// Covers intents Application-001 through Application-005.
/// </summary>
[TestFixture]
public class LearningSpaceCreateServiceTests
{
    private Mock<ILearningSpaceRepository> _mockRepository = null!;
    private LearningSpaceCreateService _sut = null!;

    // Valid test data
    private const string ValidType = "Classroom";
    private const float ValidHeight = 3.0f;
    private const float ValidWidth = 8.0f;
    private const float ValidLength = 10.0f;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningSpaceRepository>();
        _sut = new LearningSpaceCreateService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that the service successfully creates a learning space with valid input
    /// and returns the created entity with generated ID.
    /// </summary>
    [Test]
    [Description("Application-001: Verify service successfully creates a learning space with valid input")]
    public async Task CreateLearningSpaceAsync_ValidInput_ReturnsCreatedEntity()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLearningSpaceAsync(ValidType, ValidHeight, ValidWidth, ValidLength);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Type, Is.EqualTo(ValidType));
            Assert.That(result.Height, Is.EqualTo(ValidHeight));
            Assert.That(result.LearningSpaceId, Is.GreaterThan(0));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that the service propagates ArgumentException when domain validation
    /// fails for invalid type.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service propagates ArgumentException for invalid type")]
    public void CreateLearningSpaceAsync_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = "InvalidType";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.CreateLearningSpaceAsync(invalidType, ValidHeight, ValidWidth, ValidLength));
        Assert.That(ex!.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that the service propagates ArgumentException when domain validation
    /// fails for zero dimension.
    /// </summary>
    [Test]
    [Description("Application-003: Verify service propagates ArgumentException for zero dimension")]
    public void CreateLearningSpaceAsync_ZeroDimension_ThrowsArgumentException()
    {
        // Arrange
        var zeroHeight = 0.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.CreateLearningSpaceAsync(ValidType, zeroHeight, ValidWidth, ValidLength));
        Assert.That(ex!.Message, Does.Contain("Height must be positive and non-zero"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that the service propagates ArgumentException when domain validation
    /// fails for negative dimension.
    /// </summary>
    [Test]
    [Description("Application-004: Verify service propagates ArgumentException for negative dimension")]
    public void CreateLearningSpaceAsync_NegativeDimension_ThrowsArgumentException()
    {
        // Arrange
        var negativeWidth = -8.0f;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.CreateLearningSpaceAsync(ValidType, ValidHeight, negativeWidth, ValidLength));
        Assert.That(ex!.Message, Does.Contain("Width must be positive and non-zero"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service propagates repository exceptions when database
    /// operation fails.
    /// </summary>
    [Test]
    [Description("Application-005: Verify service propagates repository exceptions when database operation fails")]
    public void CreateLearningSpaceAsync_RepositoryFails_ThrowsException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CreateLearningSpaceAsync(ValidType, ValidHeight, ValidWidth, ValidLength));
        Assert.That(ex!.Message, Does.Contain("Database connection failed"));
    }
}
