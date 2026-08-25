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
    /// Application-001: Verify that the service successfully creates a learning space
    /// with valid input and returns the created entity with generated ID.
    /// </summary>
    [Test]
    [Description("Application-001: Service creates learning space with valid input and returns entity with generated ID")]
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
            Assert.That(result.Width, Is.EqualTo(ValidWidth));
            Assert.That(result.Length, Is.EqualTo(ValidLength));
            Assert.That(result.LearningSpaceId, Is.GreaterThan(0));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Once);
    }

    /// <summary>
    /// Application-002, Application-003, Application-004: Verify that the service propagates
    /// ArgumentException when domain validation fails and does not call the repository.
    /// </summary>
    [TestCase("InvalidType", 3.0f, 8.0f, 10.0f, "Type must be Classroom, Auditorium, or Laboratory",
        Description = "Application-002: Invalid type propagates ArgumentException")]
    [TestCase("Classroom", 0.0f, 8.0f, 10.0f, "Height must be positive and non-zero",
        Description = "Application-003: Zero dimension propagates ArgumentException")]
    [TestCase("Classroom", 3.0f, -8.0f, 10.0f, "Width must be positive and non-zero",
        Description = "Application-004: Negative dimension propagates ArgumentException")]
    public async Task CreateLearningSpaceAsync_DomainValidationFails_ThrowsArgumentException(
        string type, float height, float width, float length, string expectedMessage)
    {
        // Arrange
        // No repository setup needed — domain validation should fail before repository is called

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.CreateLearningSpaceAsync(type, height, width, length);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedMessage));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service propagates repository exceptions
    /// when the database operation fails.
    /// </summary>
    [Test]
    [Description("Application-005: Service propagates repository exception when database operation fails")]
    public async Task CreateLearningSpaceAsync_RepositoryFails_PropagatesException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            await _sut.CreateLearningSpaceAsync(ValidType, ValidHeight, ValidWidth, ValidLength);
        }
        catch (InvalidOperationException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected InvalidOperationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Database connection failed"));
        });
    }
}
