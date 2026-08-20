using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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
    [Description("Application-001: Successfully creates a learning space with valid input and returns entity with generated ID")]
    public async Task CreateLearningSpaceAsync_ValidInput_ReturnsCreatedEntity()
    {
        // Arrange
        var type = ValidType;
        var height = ValidHeight;
        var width = ValidWidth;
        var length = ValidLength;

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
            Assert.That(result.LearningSpaceId, Is.GreaterThan(0));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that the service propagates ArgumentException when domain validation
    /// fails for invalid type, and does not call the repository.
    /// </summary>
    [Test]
    [Description("Application-002: Invalid type propagates ArgumentException and does not call repository")]
    public void CreateLearningSpaceAsync_InvalidType_ThrowsArgumentExceptionAndDoesNotCallRepository()
    {
        // Arrange
        var invalidType = "InvalidType";

        // Act
        ArgumentException? caughtException = null;
        try
        {
            _sut.CreateLearningSpaceAsync(invalidType, ValidHeight, ValidWidth, ValidLength).Wait();
        }
        catch (AggregateException ae) when (ae.InnerException is ArgumentException argEx)
        {
            caughtException = argEx;
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that the service propagates ArgumentException when domain validation
    /// fails for zero dimension, and does not call the repository.
    /// </summary>
    [Test]
    [Description("Application-003: Zero dimension propagates ArgumentException and does not call repository")]
    public void CreateLearningSpaceAsync_ZeroDimension_ThrowsArgumentExceptionAndDoesNotCallRepository()
    {
        // Arrange
        var zeroHeight = 0.0f;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            _sut.CreateLearningSpaceAsync(ValidType, zeroHeight, ValidWidth, ValidLength).Wait();
        }
        catch (AggregateException ae) when (ae.InnerException is ArgumentException argEx)
        {
            caughtException = argEx;
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Height must be positive and non-zero"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that the service propagates ArgumentException when domain validation
    /// fails for negative dimension, and does not call the repository.
    /// </summary>
    [Test]
    [Description("Application-004: Negative dimension propagates ArgumentException and does not call repository")]
    public void CreateLearningSpaceAsync_NegativeDimension_ThrowsArgumentExceptionAndDoesNotCallRepository()
    {
        // Arrange
        var negativeWidth = -8.0f;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            _sut.CreateLearningSpaceAsync(ValidType, ValidHeight, negativeWidth, ValidLength).Wait();
        }
        catch (AggregateException ae) when (ae.InnerException is ArgumentException argEx)
        {
            caughtException = argEx;
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Width must be positive and non-zero"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningSpace>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service propagates repository exceptions when database
    /// operation fails.
    /// </summary>
    [Test]
    [Description("Application-005: Repository failure propagates InvalidOperationException")]
    public void CreateLearningSpaceAsync_RepositoryFails_ThrowsException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningSpace>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            _sut.CreateLearningSpaceAsync(ValidType, ValidHeight, ValidWidth, ValidLength).Wait();
        }
        catch (AggregateException ae) when (ae.InnerException is InvalidOperationException opEx)
        {
            caughtException = opEx;
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
