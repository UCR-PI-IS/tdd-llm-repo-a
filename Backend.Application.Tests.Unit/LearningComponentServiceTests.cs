using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService"/>.
/// Covers GetComponentsByLearningSpaceIdAsync (previous story) and
/// CreateComponentAsync (Application-001 through Application-006).
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
    }

    /// <summary>
    /// Application-001: Verify service generates a unique ID when creating a component without an ID.
    /// </summary>
    [Test]
    [Description("Application-001: Verify service generates a unique ID when creating a component without an ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            LearningSpaceId = ValidLearningSpaceId,
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10f,
            Y = 5f,
            Z = 0f,
            Orientation = "North"
        };

        _mockIdGenerator
            .Setup(g => g.GenerateIdAsync())
            .ReturnsAsync("COMP-12345");
        _mockRepository
            .Setup(r => r.ExistsAsync("COMP-12345"))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-12345"));
            Assert.That(result.Message, Does.Contain("auto-generated"));
        });
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify service uses the provided explicit ID and does not call the generator.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service uses the provided explicit ID")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            ComponentId = "COMP-99999",
            LearningSpaceId = ValidLearningSpaceId,
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10f,
            Y = 5f,
            Z = 0f,
            Orientation = "North"
        };

        _mockRepository
            .Setup(r => r.ExistsAsync("COMP-99999"))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo("COMP-99999"));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify service throws DuplicateIdException when explicit ID already exists.
    /// </summary>
    [Test]
    [Description("Application-003: Verify service throws DuplicateIdException when explicit ID already exists")]
    public void CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            ComponentId = "COMP-EXISTING",
            LearningSpaceId = ValidLearningSpaceId,
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10f,
            Y = 5f,
            Z = 0f,
            Orientation = "North"
        };

        _mockRepository
            .Setup(r => r.ExistsAsync("COMP-EXISTING"))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateIdException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex.Message, Does.Contain("COMP-EXISTING"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify service retries ID generation when the first generated ID collides.
    /// </summary>
    [Test]
    [Description("Application-004: Verify service retries ID generation on collision")]
    public async Task CreateComponentAsync_DuplicateGeneratedId_RetriesUntilUnique()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            LearningSpaceId = ValidLearningSpaceId,
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10f,
            Y = 5f,
            Z = 0f,
            Orientation = "North"
        };

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
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Exactly(2));
    }

    /// <summary>
    /// Application-005: Verify service throws ValidationException for invalid dimensions.
    /// </summary>
    [Test]
    [Description("Application-005: Verify service throws ValidationException for invalid dimensions")]
    public void CreateComponentAsync_WithInvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            LearningSpaceId = ValidLearningSpaceId,
            Width = -1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10f,
            Y = 5f,
            Z = 0f,
            Orientation = "North"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex.Message, Does.Contain("Width").IgnoreCase);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-006: Verify service throws ValidationException for invalid orientation.
    /// </summary>
    [Test]
    [Description("Application-006: Verify service throws ValidationException for invalid orientation")]
    public void CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest
        {
            LearningSpaceId = ValidLearningSpaceId,
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10f,
            Y = 5f,
            Z = 0f,
            Orientation = "InvalidDirection"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex.Message, Does.Contain("Orientation").IgnoreCase);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    #region GetComponentsByLearningSpaceIdAsync Tests (Previous Story)

    /// <summary>
    /// Verify service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
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
    /// Verify service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
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
    /// Verify service throws ArgumentException when learning space ID is null or empty.
    /// </summary>
    [TestCase("", Description = "Empty string learning space ID throws ArgumentException")]
    [TestCase(null, Description = "Null learning space ID throws ArgumentException")]
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

    #endregion
}
