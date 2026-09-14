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
/// Covers intents Application-001 through Application-006 for CreateComponentAsync,
/// plus existing tests for GetComponentsByLearningSpaceIdAsync.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private Mock<IComponentIdGenerator> _mockIdGenerator = null!;
    private LearningComponentService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";
    private const string ValidComponentId = "COMP-12345";
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

    private static CreateComponentRequest CreateRequest(string componentId = "")
    {
        return new CreateComponentRequest(
            componentId,
            ValidLearningSpaceId,
            ValidWidth,
            ValidHeight,
            ValidDepth,
            ValidX,
            ValidY,
            ValidZ,
            ValidOrientation);
    }

    #region GetComponentsByLearningSpaceIdAsync

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

    #region CreateComponentAsync

    /// <summary>
    /// Application-001: Verify that when creating a component without an ID, the service generates a unique ID.
    /// </summary>
    [Test]
    [Description("Application-001: CreateComponentAsync without ID generates unique ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var request = CreateRequest(componentId: "");
        _mockIdGenerator.Setup(g => g.GenerateIdAsync()).ReturnsAsync("COMP-12345");
        _mockRepository.Setup(r => r.ExistsAsync("COMP-12345")).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-12345"));
            _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID.
    /// </summary>
    [Test]
    [Description("Application-002: CreateComponentAsync with explicit ID uses provided ID")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var request = CreateRequest(componentId: "COMP-99999");
        _mockRepository.Setup(r => r.ExistsAsync("COMP-99999")).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-99999"));
            _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that creating a component with a duplicate explicit ID throws DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: CreateComponentAsync with duplicate explicit ID throws DuplicateIdException")]
    public void CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var request = CreateRequest(componentId: "COMP-EXISTING");
        _mockRepository.Setup(r => r.ExistsAsync("COMP-EXISTING")).ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateIdException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("COMP-EXISTING"));
    }

    /// <summary>
    /// Application-004: Verify that when ID generator produces a duplicate ID, the service retries until unique.
    /// </summary>
    [Test]
    [Description("Application-004: CreateComponentAsync retries ID generation when duplicate is produced")]
    public async Task CreateComponentAsync_IdGeneratorProducesDuplicate_RetriesUntilUnique()
    {
        // Arrange
        var request = CreateRequest(componentId: "");
        _mockIdGenerator.SetupSequence(g => g.GenerateIdAsync())
            .ReturnsAsync("COMP-EXISTING")
            .ReturnsAsync("COMP-UNIQUE");
        _mockRepository.Setup(r => r.ExistsAsync("COMP-EXISTING")).ReturnsAsync(true);
        _mockRepository.Setup(r => r.ExistsAsync("COMP-UNIQUE")).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo("COMP-UNIQUE"));
            _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Exactly(2));
        });
    }

    /// <summary>
    /// Application-005: Verify that creating a component with invalid dimensions throws ValidationException.
    /// </summary>
    [Test]
    [Description("Application-005: CreateComponentAsync with invalid dimensions throws ValidationException")]
    public void CreateComponentAsync_WithNegativeWidth_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            "", ValidLearningSpaceId, -1.5f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.Multiple(() =>
        {
            Assert.That(ex!.Message, Does.Contain("Width"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-006: Verify that creating a component with invalid orientation throws ValidationException.
    /// </summary>
    [Test]
    [Description("Application-006: CreateComponentAsync with invalid orientation throws ValidationException")]
    public void CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            "", ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "InvalidDirection");

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.Multiple(() =>
        {
            Assert.That(ex!.Message, Does.Contain("Orientation"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
        });
    }

    #endregion
}
