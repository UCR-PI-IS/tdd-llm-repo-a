using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
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

    private static CreateComponentRequest CreateValidRequestWithoutId()
    {
        return new CreateComponentRequest(
            componentId: null,
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North");
    }

    private static CreateComponentRequest CreateValidRequestWithExplicitId(string componentId)
    {
        return new CreateComponentRequest(
            componentId: componentId,
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North");
    }

    private static CreateComponentRequest CreateRequestWithNegativeWidth()
    {
        return new CreateComponentRequest(
            componentId: null,
            learningSpaceId: ValidLearningSpaceId,
            width: -1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "North");
    }

    private static CreateComponentRequest CreateRequestWithInvalidOrientation()
    {
        return new CreateComponentRequest(
            componentId: null,
            learningSpaceId: ValidLearningSpaceId,
            width: 1.5f,
            height: 1.0f,
            depth: 0.5f,
            x: 10.0f,
            y: 5.0f,
            z: 0.0f,
            orientation: "InvalidDirection");
    }

    /// <summary>
    /// Application-001: Verify that when creating a component without an ID, the service generates a unique ID.
    /// </summary>
    [Test]
    [Description("Application-001: Verify that when creating a component without an ID, the service generates a unique ID")]
    public async Task CreateComponentAsync_WithoutId_GeneratesUniqueId()
    {
        // Arrange
        var request = CreateValidRequestWithoutId();
        _mockIdGenerator.Setup(x => x.GenerateIdAsync()).ReturnsAsync(GeneratedComponentId);
        _mockRepository.Setup(x => x.ExistsAsync(GeneratedComponentId)).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo(GeneratedComponentId));
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
        var request = CreateValidRequestWithExplicitId(ExplicitComponentId);
        _mockRepository.Setup(x => x.ExistsAsync(ExplicitComponentId)).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo(ExplicitComponentId));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that when creating a component with an explicit ID that already exists, the service throws a DuplicateIdException.
    /// </summary>
    [Test]
    [Description("Application-003: Verify that creating a component with a duplicate explicit ID throws DuplicateIdException")]
    public void CreateComponentAsync_WithDuplicateExplicitId_ThrowsDuplicateIdException()
    {
        // Arrange
        var request = CreateValidRequestWithExplicitId(ExistingComponentId);
        _mockRepository.Setup(x => x.ExistsAsync(ExistingComponentId)).ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateIdException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain(ExistingComponentId));
    }

    /// <summary>
    /// Application-004: Verify that when ID generator produces an ID that already exists, the service retries until a unique ID is found.
    /// </summary>
    [Test]
    [Description("Application-004: Verify that service retries ID generation when duplicate is encountered")]
    public async Task CreateComponentAsync_GeneratedIdExists_RetriesUntilUnique()
    {
        // Arrange
        var request = CreateValidRequestWithoutId();
        _mockIdGenerator.SetupSequence(x => x.GenerateIdAsync())
            .ReturnsAsync(ExistingComponentId)
            .ReturnsAsync(UniqueComponentId);
        _mockRepository.Setup(x => x.ExistsAsync(ExistingComponentId)).ReturnsAsync(true);
        _mockRepository.Setup(x => x.ExistsAsync(UniqueComponentId)).ReturnsAsync(false);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<LearningComponent>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateComponentAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ComponentId, Is.EqualTo(UniqueComponentId));
            _mockIdGenerator.Verify(x => x.GenerateIdAsync(), Times.Exactly(2));
        });
    }

    /// <summary>
    /// Application-005: Verify that creating a component with invalid dimensions throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-005: Verify that creating a component with invalid dimensions throws ValidationException")]
    public void CreateComponentAsync_WithInvalidDimensions_ThrowsValidationException()
    {
        // Arrange
        var request = CreateRequestWithNegativeWidth();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("Width"));
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }

    /// <summary>
    /// Application-006: Verify that creating a component with invalid orientation throws a ValidationException.
    /// </summary>
    [Test]
    [Description("Application-006: Verify that creating a component with invalid orientation throws ValidationException")]
    public void CreateComponentAsync_WithInvalidOrientation_ThrowsValidationException()
    {
        // Arrange
        var request = CreateRequestWithInvalidOrientation();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateComponentAsync(request));
        Assert.That(ex!.Message, Does.Contain("Orientation"));
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<LearningComponent>()), Times.Never);
    }
}
