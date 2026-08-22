using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningSpaceCreateService.CreateLearningSpaceAsync"/>.
/// Covers intents Application-001 through Application-005 for story SQL-LS-001-007.
/// </summary>
[TestFixture]
public class LearningSpaceCreateServiceTests
{
    private Mock<ILearningSpaceRepository> _mockRepository = null!;
    private LearningSpaceCreateService _sut = null!;

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
    /// Application-001: Verify that the service successfully creates a learning space
    /// with valid input, returns the created entity with generated ID, and calls the repository.
    /// </summary>
    [Test]
    [Description("Application-001: Valid input creates learning space and persists via repository")]
    public async Task CreateLearningSpaceAsync_ValidInput_ReturnsCreatedEntityAndPersists()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLearningSpaceAsync(type, height, width, length);

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
    /// Application-002, Application-003, Application-004: Verify that the service propagates
    /// ArgumentException when domain validation fails (invalid type, zero dimension, or negative
    /// dimension) and does not call the repository.
    /// </summary>
    [TestCase("InvalidType", 3.0f, 8.0f, 10.0f, "Type must be Classroom, Auditorium, or Laboratory",
        Description = "Application-002: Invalid type propagates ArgumentException and does not persist")]
    [TestCase("Classroom", 0.0f, 8.0f, 10.0f, "Height must be positive and non-zero",
        Description = "Application-003: Zero height propagates ArgumentException and does not persist")]
    [TestCase("Classroom", 3.0f, -8.0f, 10.0f, "Width must be positive and non-zero",
        Description = "Application-004: Negative width propagates ArgumentException and does not persist")]
    public async Task CreateLearningSpaceAsync_DomainValidationFails_ThrowsArgumentExceptionAndDoesNotPersist(
        string type, float height, float width, float length, string expectedMessage)
    {
        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.CreateLearningSpaceAsync(type, height, width, length));
        Assert.That(ex!.Message, Does.Contain(expectedMessage));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service propagates repository exceptions
    /// when the database operation fails.
    /// </summary>
    [Test]
    [Description("Application-005: Repository failure propagates InvalidOperationException")]
    public async Task CreateLearningSpaceAsync_RepositoryFails_PropagatesException()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CreateLearningSpaceAsync(type, height, width, length));
        Assert.That(ex!.Message, Does.Contain("Database connection failed"));
    }
}
