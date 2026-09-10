using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService"/>.
/// Covers GetComponentsByLearningSpaceIdAsync (original intents) and
/// CreateComponentAsync (CPD-LC-001-009 intents Application-001 through Application-006).
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

    // ──────────────────────────────────────────────────────────────────────────
    // CreateComponentAsync tests (CPD-LC-001-009: Application-001 through Application-006)
    // ──────────────────────────────────────────────────────────────────────────

    private static CreateComponentRequest CreateValidRequest(string? componentId = null)
    {
        return new CreateComponentRequest
        {
            ComponentId = componentId,
            LearningSpaceId = "LS-001",
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "North"
        };
    }

    /// <summary>
    /// Application-001 (CPD-LC-001-009): Verify that when creating a component without
    /// an ID, the service generates a unique ID via the IComponentIdGenerator.
    /// </summary>
    [Test]
    [Description("Application-001: Service generates a unique ID when request has no ComponentId")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var generatedId = "COMP-12345";
        var request = CreateValidRequest();

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
    }

    /// <summary>
    /// Application-002 (CPD-LC-001-009): Verify that when creating a component with an
    /// explicit ID, the service uses the provided ID and does NOT call the ID generator.
    /// </summary>
    [Test]
    [Description("Application-002: Service uses explicit ID and does not call ID generator")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-99999";
        var request = CreateValidRequest(componentId: explicitId);

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
    }

    /// <summary>
    /// Application-003 (CPD-LC-001-009): Verify that when creating a component with an
    /// explicit ID that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: Duplicate explicit ID throws DuplicateIdException")]
    public async Task CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var duplicateId = "COMP-EXISTING";
        var request = CreateValidRequest(componentId: duplicateId);

        _mockRepository
            .Setup(r => r.ExistsAsync(duplicateId))
            .ReturnsAsync(true);

        // Act & Assert
        var caughtException = await Assert.ThrowsAsync<DuplicateIdException>(
            () => _sut.CreateComponentAsync(request));

        Assert.That(caughtException!.Message, Does.Contain(duplicateId));
    }

    /// <summary>
    /// Application-004 (CPD-LC-001-009): Verify that when the ID generator produces an
    /// ID that already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004: Service retries ID generation when generated ID already exists")]
    public async Task CreateComponentAsync_WhenGeneratedIdExists_RetriesUntilUnique()
    {
        // Arrange
        var request = CreateValidRequest();

        _mockIdGenerator
            .SetupSequence(g => g.GenerateIdAsync())
            .ReturnsAsync("COMP-EXISTING")
            .ReturnsAsync("COMP-UNIQUE");

        _mockRepository
            .Setup(r => r.ExistsAsync("COMP-EXISTING"))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.ExistsAsync("COMP-UNIQUE"))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo("COMP-UNIQUE"));
    }

    /// <summary>
    /// Application-005 (CPD-LC-001-009): Verify that creating a component with invalid
    /// dimensions (negative width) throws a ValidationException and does not persist.
    /// </summary>
    [Test]
    [Description("Application-005: Invalid dimensions throw ValidationException and prevent persistence")]
    public async Task CreateComponentAsync_WithNegativeWidth_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            LearningSpaceId = "LS-001",
            Width = -1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "North"
        };

        // Act & Assert
        var caughtException = await Assert.ThrowsAsync<ValidationException>(
            () => _sut.CreateComponentAsync(request));

        Assert.That(caughtException!.Message, Does.Contain("Width"));
    }

    /// <summary>
    /// Application-006 (CPD-LC-001-009): Verify that creating a component with an invalid
    /// orientation throws a ValidationException and does not persist.
    /// </summary>
    [Test]
    [Description("Application-006: Invalid orientation throws ValidationException and prevents persistence")]
    public async Task CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            LearningSpaceId = "LS-001",
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "InvalidDirection"
        };

        // Act & Assert
        var caughtException = await Assert.ThrowsAsync<ValidationException>(
            () => _sut.CreateComponentAsync(request));

        Assert.That(caughtException!.Message, Does.Contain("Orientation"));
    }
}
