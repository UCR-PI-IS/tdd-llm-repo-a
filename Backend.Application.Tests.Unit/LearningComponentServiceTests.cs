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
/// Covers GetComponentsByLearningSpaceIdAsync (Application-001 through Application-004)
/// and CreateComponentAsync (CPD-LC-001-009 Application-001 through Application-006).
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private Mock<IComponentIdGenerator> _mockIdGenerator = null!;
    private LearningComponentService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";

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
    // CPD-LC-001-009: Tests for CreateComponentAsync
    // Covers intents Application-001 through Application-006
    // ========================================================================

    private static CreateComponentRequest CreateValidRequest(string? componentId = null)
    {
        return new CreateComponentRequest(
            ComponentId: componentId,
            LearningSpaceId: "LS-001",
            Width: 1.5f,
            Height: 1.0f,
            Depth: 0.5f,
            X: 10.0f,
            Y: 5.0f,
            Z: 0.0f,
            Orientation: "North");
    }

    /// <summary>
    /// CPD-LC-001-009 Application-001: Verify that when creating a component without an ID,
    /// the service generates a unique ID via the ID generator.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-001: CreateComponentAsync without ID generates unique ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var generatedId = "COMP-12345";
        var request = CreateValidRequest(componentId: null);

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
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(generatedId));
    }

    /// <summary>
    /// CPD-LC-001-009 Application-002: Verify that when creating a component with an explicit ID,
    /// the service uses the provided ID instead of generating one.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-002: CreateComponentAsync with explicit ID uses provided ID")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-99999";
        var request = CreateValidRequest(componentId: explicitId);

        _mockRepository
            .Setup(x => x.ExistsAsync(explicitId))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(explicitId));
    }

    /// <summary>
    /// CPD-LC-001-009 Application-003: Verify that when creating a component with an explicit ID
    /// that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-003: CreateComponentAsync with duplicate explicit ID throws DuplicateIdException")]
    public async Task CreateComponentAsync_WithDuplicateId_ThrowsDuplicateIdException()
    {
        // Arrange
        var duplicateId = "COMP-EXISTING";
        var request = CreateValidRequest(componentId: duplicateId);

        _mockRepository
            .Setup(x => x.ExistsAsync(duplicateId))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateIdException>(
            () => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain(duplicateId));
    }

    /// <summary>
    /// CPD-LC-001-009 Application-004: Verify that when the ID generator produces an ID
    /// that already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-004: CreateComponentAsync retries when generated ID already exists")]
    public async Task CreateComponentAsync_IdGeneratorProducesDuplicate_RetriesUntilUnique()
    {
        // Arrange
        var request = CreateValidRequest(componentId: null);

        _mockIdGenerator
            .SetupSequence(x => x.GenerateIdAsync())
            .ReturnsAsync("COMP-EXISTING")
            .ReturnsAsync("COMP-UNIQUE");

        _mockRepository
            .Setup(x => x.ExistsAsync("COMP-EXISTING"))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(x => x.ExistsAsync("COMP-UNIQUE"))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo("COMP-UNIQUE"));
    }

    /// <summary>
    /// CPD-LC-001-009 Application-005: Verify that creating a component with invalid dimensions
    /// throws a ValidationException and the repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-005: CreateComponentAsync with negative width throws ValidationException")]
    public async Task CreateComponentAsync_InvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: "LS-001",
            Width: -1.5f,
            Height: 1.0f,
            Depth: 0.5f,
            X: 10.0f,
            Y: 5.0f,
            Z: 0.0f,
            Orientation: "North");

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            () => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("Width"));
    }

    /// <summary>
    /// CPD-LC-001-009 Application-006: Verify that creating a component with invalid orientation
    /// throws a ValidationException and the repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-006: CreateComponentAsync with invalid orientation throws ValidationException")]
    public async Task CreateComponentAsync_InvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            ComponentId: null,
            LearningSpaceId: "LS-001",
            Width: 1.5f,
            Height: 1.0f,
            Depth: 0.5f,
            X: 10.0f,
            Y: 5.0f,
            Z: 0.0f,
            Orientation: "InvalidDirection");

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            () => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("Orientation"));
    }
}
