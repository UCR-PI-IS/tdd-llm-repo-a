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
/// Covers intents Application-001 through Application-004 (GetComponentsByLearningSpaceIdAsync)
/// and CPD-LC-001-009 Application-001 through Application-006 (CreateComponentAsync).
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
    // Tests for CPD-LC-001-009: CreateComponentAsync
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
    [Description("CPD-LC-001-009 Application-001: Service generates unique ID when none provided")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var request = CreateValidRequest(componentId: null);
        _mockIdGenerator.Setup(x => x.GenerateIdAsync()).ReturnsAsync("COMP-12345");
        _mockRepository.Setup(x => x.ExistsAsync("COMP-12345")).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-12345"));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Once);
        });
    }

    /// <summary>
    /// CPD-LC-001-009 Application-002: Verify that when creating a component with an explicit ID,
    /// the service uses the provided ID instead of generating one.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-002: Service uses explicit ID when provided")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var request = CreateValidRequest(componentId: "COMP-99999");
        _mockRepository.Setup(x => x.ExistsAsync("COMP-99999")).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-99999"));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Never);
        });
    }

    /// <summary>
    /// CPD-LC-001-009 Application-003: Verify that when creating a component with an explicit ID
    /// that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-003: Duplicate explicit ID throws DuplicateIdException")]
    public async Task CreateComponentAsync_DuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var request = CreateValidRequest(componentId: "COMP-EXISTING");
        _mockRepository.Setup(x => x.ExistsAsync("COMP-EXISTING")).ReturnsAsync(true);

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
            Assert.That(caughtException!.Message, Does.Contain("COMP-EXISTING"));
        });
    }

    /// <summary>
    /// CPD-LC-001-009 Application-004: Verify that when the ID generator produces an ID that
    /// already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-004: Service retries ID generation on collision")]
    public async Task CreateComponentAsync_IdGeneratorCollision_RetriesUntilUnique()
    {
        // Arrange
        var request = CreateValidRequest(componentId: null);
        _mockIdGenerator.SetupSequence(x => x.GenerateIdAsync())
            .ReturnsAsync("COMP-EXISTING")
            .ReturnsAsync("COMP-UNIQUE");
        _mockRepository.Setup(x => x.ExistsAsync("COMP-EXISTING")).ReturnsAsync(true);
        _mockRepository.Setup(x => x.ExistsAsync("COMP-UNIQUE")).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-UNIQUE"));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Exactly(2));
        });
    }

    /// <summary>
    /// CPD-LC-001-009 Application-005: Verify that creating a component with invalid dimensions
    /// throws a ValidationException and the repository is never called.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-005: Invalid dimensions throw ValidationException")]
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
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
        });
    }

    /// <summary>
    /// CPD-LC-001-009 Application-006: Verify that creating a component with invalid orientation
    /// throws a ValidationException and the repository is never called.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Application-006: Invalid orientation throws ValidationException")]
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
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
        });
    }
}
