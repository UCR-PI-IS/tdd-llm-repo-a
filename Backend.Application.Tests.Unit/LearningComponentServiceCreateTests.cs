using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService.CreateComponentAsync"/>.
/// Covers intents Application-001 through Application-006 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentServiceCreateTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private Mock<IComponentIdGenerator> _mockIdGenerator = null!;
    private LearningComponentService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidWidth = 1.5f;
    private const float ValidHeight = 1.0f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 5.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _mockIdGenerator = new Mock<IComponentIdGenerator>();
        _sut = new LearningComponentService(_mockRepository.Object, _mockIdGenerator.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
        _mockIdGenerator.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that when creating a component without an ID, the service generates a unique ID.
    /// </summary>
    [Test]
    [Description("Application-001: Verify that when creating a component without an ID, the service generates a unique ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var generatedId = "COMP-12345";
        _mockIdGenerator
            .Setup(x => x.GenerateIdAsync())
            .ReturnsAsync(generatedId);
        _mockRepository
            .Setup(x => x.ExistsAsync(generatedId))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        var request = new CreateComponentRequest(
            ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo(generatedId));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID instead of generating one.
    /// </summary>
    [Test]
    [Description("Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-99999";
        _mockRepository
            .Setup(x => x.ExistsAsync(explicitId))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        var request = new CreateComponentRequest(
            explicitId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo(explicitId));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that when creating a component with an explicit ID that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: Verify that when creating a component with an explicit ID that already exists, the service throws a DuplicateIdException")]
    public void CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var existingId = "COMP-EXISTING";
        _mockRepository
            .Setup(x => x.ExistsAsync(existingId))
            .ReturnsAsync(true);

        var request = new CreateComponentRequest(
            existingId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation);

        // Act & Assert
        DuplicateIdException? caughtException = null;
        try
        {
            _sut.CreateComponentAsync(request).Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is DuplicateIdException)
        {
            caughtException = ex.InnerException as DuplicateIdException;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DuplicateIdException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(existingId));
        });
    }

    /// <summary>
    /// Application-004: Verify that when ID generator produces an ID that already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004: Verify that when ID generator produces an ID that already exists, the service retries until a unique ID is found")]
    public async Task CreateComponentAsync_GeneratedIdExists_RetriesUntilUnique()
    {
        // Arrange
        var existingId = "COMP-EXISTING";
        var uniqueId = "COMP-UNIQUE";
        _mockIdGenerator
            .SetupSequence(x => x.GenerateIdAsync())
            .ReturnsAsync(existingId)
            .ReturnsAsync(uniqueId);
        _mockRepository
            .Setup(x => x.ExistsAsync(existingId))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(x => x.ExistsAsync(uniqueId))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        var request = new CreateComponentRequest(
            ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo(uniqueId));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Exactly(2));
        });
    }

    /// <summary>
    /// Application-005: Verify that creating a component with invalid dimensions throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-005: Verify that creating a component with invalid dimensions throws a ValidationException")]
    public void CreateComponentAsync_InvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            ValidLearningSpaceId, -1.5f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation);

        // Act & Assert
        ValidationException? caughtException = null;
        try
        {
            _sut.CreateComponentAsync(request).Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is ValidationException)
        {
            caughtException = ex.InnerException as ValidationException;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Width"));
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-006: Verify that creating a component with invalid orientation throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-006: Verify that creating a component with invalid orientation throws a ValidationException")]
    public void CreateComponentAsync_InvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "InvalidOrientation");

        // Act & Assert
        ValidationException? caughtException = null;
        try
        {
            _sut.CreateComponentAsync(request).Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is ValidationException)
        {
            caughtException = ex.InnerException as ValidationException;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Orientation"));
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
        });
    }
}
