using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService"/>.
/// Covers GetComponentsByLearningSpaceIdAsync and CreateComponentAsync methods.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private Mock<IIdGenerator> _mockIdGenerator = null!;
    private LearningComponentService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _mockIdGenerator = new Mock<IIdGenerator>();
        _sut = new LearningComponentService(_mockRepository.Object, _mockIdGenerator.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
        _mockIdGenerator.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Application-001: Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.5f, 1.5f, 0.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 3.0f, 2.0f, 1.0f, 15f, 25f, 0f, "South")
        };

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    /// <summary>
    /// Application-002: Verify service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var emptyComponents = new List<LearningComponent>();

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Has.Count.EqualTo(0));
        });
    }

    /// <summary>
    /// Application-003 and Application-004: Verify service throws ArgumentException when
    /// learning space ID is null or empty.
    /// </summary>
    [TestCase("", Description = "Application-003: Empty string learning space ID throws ArgumentException")]
    [TestCase(null, Description = "Application-004: Null learning space ID throws ArgumentException")]
    public async Task GetComponentsByLearningSpaceIdAsync_InvalidId_ThrowsArgumentException(string? invalidLearningSpaceId)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId!);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("learningSpaceId"));
        });
    }

    // ========================================================================
    // Tests for CPD-LC-001-009: CreateComponentAsync
    // ========================================================================

    /// <summary>
    /// Application-001 (CPD-LC-001-009): Verify that when creating a component without an ID,
    /// the service generates a unique ID using the ID generator.
    /// </summary>
    [Test]
    [Description("Application-001 (CPD-LC-001-009): Service generates unique ID when none provided")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var generatedId = "COMP-12345";
        var createRequest = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: "LS-001",
            Width: 1.5f, Height: 1.0f, Depth: 0.5f,
            X: 10.0f, Y: 5.0f, Z: 0.0f,
            Orientation: "North");

        _mockIdGenerator
            .Setup(x => x.GenerateIdAsync())
            .ReturnsAsync(generatedId);

        _mockRepository
            .Setup(x => x.ExistsAsync(generatedId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(createRequest);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(generatedId));
        _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Once);
    }

    /// <summary>
    /// Application-002 (CPD-LC-001-009): Verify that when creating a component with an explicit ID,
    /// the service uses the provided ID instead of generating one.
    /// </summary>
    [Test]
    [Description("Application-002 (CPD-LC-001-009): Service uses explicit ID when provided")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-99999";
        var createRequest = new CreateComponentRequest(
            ComponentId: explicitId,
            LearningSpaceId: "LS-001",
            Width: 1.5f, Height: 1.0f, Depth: 0.5f,
            X: 10.0f, Y: 5.0f, Z: 0.0f,
            Orientation: "North");

        _mockRepository
            .Setup(x => x.ExistsAsync(explicitId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(createRequest);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(explicitId));
        _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Never);
    }

    /// <summary>
    /// Application-003 (CPD-LC-001-009): Verify that when creating a component with an explicit ID
    /// that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003 (CPD-LC-001-009): Duplicate explicit ID throws DuplicateIdException")]
    public async Task CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var duplicateId = "COMP-EXISTING";
        var createRequest = new CreateComponentRequest(
            ComponentId: duplicateId,
            LearningSpaceId: "LS-001",
            Width: 1.5f, Height: 1.0f, Depth: 0.5f,
            X: 10.0f, Y: 5.0f, Z: 0.0f,
            Orientation: "North");

        _mockRepository
            .Setup(x => x.ExistsAsync(duplicateId))
            .ReturnsAsync(true);

        // Act & Assert
        DuplicateIdException? caughtException = null;
        try
        {
            await _sut.CreateComponentAsync(createRequest);
        }
        catch (DuplicateIdException ex)
        {
            caughtException = ex;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DuplicateIdException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(duplicateId));
        });
    }

    /// <summary>
    /// Application-004 (CPD-LC-001-009): Verify that when the ID generator produces an ID
    /// that already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004 (CPD-LC-001-009): Service retries when generated ID already exists")]
    public async Task CreateComponentAsync_WhenIdGeneratorProducesDuplicate_RetriesUntilUnique()
    {
        // Arrange
        var existingId = "COMP-EXISTING";
        var uniqueId = "COMP-UNIQUE";
        var createRequest = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: "LS-001",
            Width: 1.5f, Height: 1.0f, Depth: 0.5f,
            X: 10.0f, Y: 5.0f, Z: 0.0f,
            Orientation: "North");

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

        // Act
        var result = await _sut.CreateComponentAsync(createRequest);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(uniqueId));
        _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Exactly(2));
    }

    /// <summary>
    /// Application-005 (CPD-LC-001-009): Verify that creating a component with invalid dimensions
    /// throws a ValidationException and the repository is never called.
    /// </summary>
    [Test]
    [Description("Application-005 (CPD-LC-001-009): Invalid dimensions throw ValidationException")]
    public async Task CreateComponentAsync_WithInvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var createRequest = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: "LS-001",
            Width: -1.5f, Height: 1.0f, Depth: 0.5f,
            X: 10.0f, Y: 5.0f, Z: 0.0f,
            Orientation: "North");

        // Act & Assert
        ValidationException? caughtException = null;
        try
        {
            await _sut.CreateComponentAsync(createRequest);
        }
        catch (ValidationException ex)
        {
            caughtException = ex;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Width"));
        });
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-006 (CPD-LC-001-009): Verify that creating a component with invalid orientation
    /// throws a ValidationException and the repository is never called.
    /// </summary>
    [Test]
    [Description("Application-006 (CPD-LC-001-009): Invalid orientation throws ValidationException")]
    public async Task CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var createRequest = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: "LS-001",
            Width: 1.5f, Height: 1.0f, Depth: 0.5f,
            X: 10.0f, Y: 5.0f, Z: 0.0f,
            Orientation: "InvalidDirection");

        // Act & Assert
        ValidationException? caughtException = null;
        try
        {
            await _sut.CreateComponentAsync(createRequest);
        }
        catch (ValidationException ex)
        {
            caughtException = ex;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Orientation"));
        });
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }
}
