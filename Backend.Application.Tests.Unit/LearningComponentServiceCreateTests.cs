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
/// Unit tests for <see cref="LearningComponentService.CreateComponentAsync"/>.
/// Covers intents Application-001 through Application-006 for story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentServiceCreateTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private Mock<IComponentIdGenerator> _idGenerator = null!;
    private LearningComponentService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _idGenerator = new Mock<IComponentIdGenerator>();
        _service = new LearningComponentService(_mockRepository.Object, _idGenerator.Object);
    }

    /// <summary>
    /// Application-001: Verify that when creating a component without an ID, the service generates a unique ID.
    /// </summary>
    [Test]
    [Description("Application-001: Verify that when creating a component without an ID, the service generates a unique ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        _idGenerator.Setup(x => x.GenerateIdAsync()).ReturnsAsync("COMP-12345");
        var createRequest = new CreateComponentRequest
        {
            LearningSpaceId = "LS-001",
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "North"
        };

        // Act
        var result = await _service.CreateComponentAsync(createRequest);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo("COMP-12345"));
        _idGenerator.Verify(x => x.GenerateIdAsync(), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID instead of generating one.
    /// </summary>
    [Test]
    [Description("Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var createRequestWithExplicitId = new CreateComponentRequest
        {
            ComponentId = "COMP-99999",
            LearningSpaceId = "LS-001",
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "North"
        };

        // Act
        var result = await _service.CreateComponentAsync(createRequestWithExplicitId);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo("COMP-99999"));
        _idGenerator.Verify(x => x.GenerateIdAsync(), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that when creating a component with an explicit ID that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: Verify that when creating a component with an explicit ID that already exists, the service throws a DuplicateIdException")]
    public async Task CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        _mockRepository.Setup(x => x.ExistsAsync("COMP-EXISTING")).ReturnsAsync(true);
        var createRequestWithDuplicateId = new CreateComponentRequest
        {
            ComponentId = "COMP-EXISTING",
            LearningSpaceId = "LS-001",
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "North"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateIdException>(() => _service.CreateComponentAsync(createRequestWithDuplicateId));
        Assert.That(ex!.Message, Does.Contain("COMP-EXISTING"));
    }

    /// <summary>
    /// Application-004: Verify that when ID generator produces an ID that already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004: Verify that when ID generator produces an ID that already exists, the service retries")]
    public async Task CreateComponentAsync_DuplicateGeneratedId_RetriesUntilUnique()
    {
        // Arrange
        _idGenerator.SetupSequence(x => x.GenerateIdAsync()).ReturnsAsync("COMP-EXISTING").ReturnsAsync("COMP-UNIQUE");
        _mockRepository.Setup(x => x.ExistsAsync("COMP-EXISTING")).ReturnsAsync(true);
        _mockRepository.Setup(x => x.ExistsAsync("COMP-UNIQUE")).ReturnsAsync(false);
        var createRequest = new CreateComponentRequest
        {
            LearningSpaceId = "LS-001",
            Width = 1.5f,
            Height = 1.0f,
            Depth = 0.5f,
            X = 10.0f,
            Y = 5.0f,
            Z = 0.0f,
            Orientation = "North"
        };

        // Act
        var result = await _service.CreateComponentAsync(createRequest);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo("COMP-UNIQUE"));
        _idGenerator.Verify(x => x.GenerateIdAsync(), Times.Exactly(2));
    }

    /// <summary>
    /// Application-005: Verify that creating a component with invalid dimensions throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-005: Verify that creating a component with invalid dimensions throws a ValidationException")]
    public async Task CreateComponentAsync_WithInvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var createRequestWithNegativeWidth = new CreateComponentRequest
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
        var ex = Assert.ThrowsAsync<ValidationException>(() => _service.CreateComponentAsync(createRequestWithNegativeWidth));
        Assert.That(ex!.Message, Does.Contain("Width"));
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-006: Verify that creating a component with invalid orientation throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-006: Verify that creating a component with invalid orientation throws a ValidationException")]
    public async Task CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var createRequestWithInvalidOrientation = new CreateComponentRequest
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
        var ex = Assert.ThrowsAsync<ValidationException>(() => _service.CreateComponentAsync(createRequestWithInvalidOrientation));
        Assert.That(ex!.Message, Does.Contain("Orientation"));
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }
}
