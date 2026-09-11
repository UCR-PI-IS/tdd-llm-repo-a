using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ILearningComponentService.CreateComponentAsync"/>.
/// Covers intents Application-001 through Application-006 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentServiceCreateTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private Mock<IComponentIdGenerator> _mockIdGenerator = null!;
    private ILearningComponentService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "LS-001";
    private const string GeneratedComponentId = "COMP-12345";
    private const string ExplicitComponentId = "COMP-99999";
    private const string ExistingComponentId = "COMP-EXISTING";
    private const string UniqueComponentId = "COMP-UNIQUE";

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
    [Description("Application-001: Verify that service generates unique ID when creating component without explicit ID")]
    public async Task CreateComponentAsync_WithoutExplicitId_GeneratesUniqueId()
    {
        // Arrange
        var request = new CreateComponentRequest(
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North",
            componentId: null);

        _mockIdGenerator
            .Setup(g => g.GenerateIdAsync())
            .ReturnsAsync(GeneratedComponentId);

        _mockRepository
            .Setup(r => r.ExistsAsync(GeneratedComponentId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(GeneratedComponentId));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that when creating a component with an explicit ID, the service uses the provided ID.
    /// </summary>
    [Test]
    [Description("Application-002: Verify that service uses explicit ID when provided")]
    public async Task CreateComponentAsync_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var request = new CreateComponentRequest(
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North",
            componentId: ExplicitComponentId);

        _mockRepository
            .Setup(r => r.ExistsAsync(ExplicitComponentId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(ExplicitComponentId));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that when creating a component with an explicit ID that already exists, 
    /// the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: Verify that DuplicateIdException is thrown for existing explicit ID")]
    public void CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North",
            componentId: ExistingComponentId);

        _mockRepository
            .Setup(r => r.ExistsAsync(ExistingComponentId))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateIdException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain(ExistingComponentId));
    }

    /// <summary>
    /// Application-004: Verify that when ID generator produces an ID that already exists, 
    /// the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004: Verify that service retries ID generation until unique ID is found")]
    public async Task CreateComponentAsync_WithInitialDuplicateId_RetriesUntilUnique()
    {
        // Arrange
        var request = new CreateComponentRequest(
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North",
            componentId: null);

        _mockIdGenerator
            .SetupSequence(g => g.GenerateIdAsync())
            .ReturnsAsync(ExistingComponentId)
            .ReturnsAsync(UniqueComponentId);

        _mockRepository
            .Setup(r => r.ExistsAsync(ExistingComponentId))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.ExistsAsync(UniqueComponentId))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<LearningComponent>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.That(result.ComponentId, Is.EqualTo(UniqueComponentId));
        _mockIdGenerator.Verify(g => g.GenerateIdAsync(), Times.Exactly(2));
    }

    /// <summary>
    /// Application-005: Verify that creating a component with invalid dimensions throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-005: Verify that ValidationException is thrown for invalid dimensions")]
    public void CreateComponentAsync_WithNegativeWidth_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            learningSpaceId: ValidLearningSpaceId,
            width: -1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North",
            componentId: null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("Width").Or.Contain("width"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-006: Verify that creating a component with invalid orientation throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-006: Verify that ValidationException is thrown for invalid orientation")]
    public void CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateComponentRequest(
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "InvalidDirection",
            componentId: null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("Orientation").Or.Contain("orientation"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }
}
