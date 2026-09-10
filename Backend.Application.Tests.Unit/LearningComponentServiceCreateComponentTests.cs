using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService.CreateComponentAsync"/>.
/// Covers intents Application-001 through Application-006 from story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentServiceCreateComponentTests
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

    /// <summary>
    /// Application-001: Verify that when creating a component without an ID,
    /// the service generates a unique ID via the ID generator.
    /// </summary>
    [Test]
    [Description("Application-001: Verify that when creating a component without an ID, the service generates a unique ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var generatedId = "COMP-12345";
        var request = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: ValidLearningSpaceId,
            Width: ValidWidth,
            Height: ValidHeight,
            Depth: ValidDepth,
            X: ValidX,
            Y: ValidY,
            Z: ValidZ,
            Orientation: ValidOrientation);

        _mockIdGenerator
            .Setup(g => g.GenerateIdAsync())
            .ReturnsAsync(generatedId);

        _mockRepository
            .Setup(r => r.ExistsAsync(generatedId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(generatedId));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that when creating a component with an explicit ID,
    /// the service uses the provided ID instead of generating one.
    /// </summary>
    [Test]
    [Description("Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-99999";
        var request = new CreateComponentRequest(
            ComponentId: explicitId,
            LearningSpaceId: ValidLearningSpaceId,
            Width: ValidWidth,
            Height: ValidHeight,
            Depth: ValidDepth,
            X: ValidX,
            Y: ValidY,
            Z: ValidZ,
            Orientation: ValidOrientation);

        _mockRepository
            .Setup(r => r.ExistsAsync(explicitId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(explicitId));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that when creating a component with an explicit ID
    /// that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: Verify that creating a component with a duplicate explicit ID throws DuplicateIdException")]
    public async Task CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var duplicateId = "COMP-EXISTING";
        var request = new CreateComponentRequest(
            ComponentId: duplicateId,
            LearningSpaceId: ValidLearningSpaceId,
            Width: ValidWidth,
            Height: ValidHeight,
            Depth: ValidDepth,
            X: ValidX,
            Y: ValidY,
            Z: ValidZ,
            Orientation: ValidOrientation);

        _mockRepository
            .Setup(r => r.ExistsAsync(duplicateId))
            .ReturnsAsync(true);

        // Act
        DuplicateIdException? caughtException = null;
        try
        {
            await _sut.CreateComponentAsync(request);
        }
        catch (DuplicateIdException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DuplicateIdException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(duplicateId));
        });
    }

    /// <summary>
    /// Application-004: Verify that when the ID generator produces an ID that already exists,
    /// the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004: Verify that the service retries ID generation when a generated ID already exists")]
    public async Task CreateComponentAsync_WhenGeneratedIdExists_RetriesUntilUnique()
    {
        // Arrange
        var existingId = "COMP-EXISTING";
        var uniqueId = "COMP-UNIQUE";
        var request = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: ValidLearningSpaceId,
            Width: ValidWidth,
            Height: ValidHeight,
            Depth: ValidDepth,
            X: ValidX,
            Y: ValidY,
            Z: ValidZ,
            Orientation: ValidOrientation);

        _mockIdGenerator
            .SetupSequence(g => g.GenerateIdAsync())
            .ReturnsAsync(existingId)
            .ReturnsAsync(uniqueId);

        _mockRepository
            .Setup(r => r.ExistsAsync(existingId))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.ExistsAsync(uniqueId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(uniqueId));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Exactly(2));
    }

    /// <summary>
    /// Application-005: Verify that creating a component with invalid dimensions
    /// (negative width) throws a ValidationException and does not persist the component.
    /// </summary>
    [Test]
    [Description("Application-005: Verify that creating a component with invalid dimensions throws ValidationException")]
    public async Task CreateComponentAsync_WithInvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: ValidLearningSpaceId,
            Width: -1.5f,
            Height: ValidHeight,
            Depth: ValidDepth,
            X: ValidX,
            Y: ValidY,
            Z: ValidZ,
            Orientation: ValidOrientation);

        // Act
        ValidationException? caughtException = null;
        try
        {
            await _sut.CreateComponentAsync(request);
        }
        catch (ValidationException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Width"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-006: Verify that creating a component with an invalid orientation
    /// throws a ValidationException and does not persist the component.
    /// </summary>
    [Test]
    [Description("Application-006: Verify that creating a component with invalid orientation throws ValidationException")]
    public async Task CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: ValidLearningSpaceId,
            Width: ValidWidth,
            Height: ValidHeight,
            Depth: ValidDepth,
            X: ValidX,
            Y: ValidY,
            Z: ValidZ,
            Orientation: "InvalidDirection");

        // Act
        ValidationException? caughtException = null;
        try
        {
            await _sut.CreateComponentAsync(request);
        }
        catch (ValidationException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Orientation"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }
}
