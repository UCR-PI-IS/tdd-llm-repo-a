using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="WhiteboardService.CreateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-004.
/// </summary>
[TestFixture]
public class WhiteboardServiceTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepository = null!;
    private Mock<ILearningSpaceRepository> _mockLearningSpaceRepository = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceRepository = new Mock<ILearningSpaceRepository>();
        _sut = new WhiteboardService(_mockWhiteboardRepository.Object, _mockLearningSpaceRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepository.VerifyAll();
        _mockLearningSpaceRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify service successfully creates whiteboard when it fits in learning space.
    /// </summary>
    [Test]
    [Description("Application-001: Verify service successfully creates whiteboard when it fits in learning space")]
    public async Task CreateWhiteboardAsync_ValidRequest_ReturnsCreatedWhiteboard()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var learningSpace = new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);
        
        var request = new CreateWhiteboardRequest(
            ValidComponentId, learningSpaceId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepository
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(result.MarkerColor, Is.EqualTo(ValidMarkerColor));
        });
        _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify service throws NotFoundException when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service throws NotFoundException when learning space does not exist")]
    public void CreateWhiteboardAsync_LearningSpaceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var learningSpaceId = "NON-EXISTENT";
        
        var request = new CreateWhiteboardRequest(
            ValidComponentId, learningSpaceId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync((LearningSpace?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateWhiteboardAsync(request));
        Assert.That(ex!.Message, Does.Contain("Learning space not found"));
    }

    /// <summary>
    /// Application-003: Verify service throws ValidationException when whiteboard doesn't fit in learning space.
    /// </summary>
    [Test]
    [Description("Application-003: Verify service throws ValidationException when whiteboard doesn't fit")]
    public void CreateWhiteboardAsync_WhiteboardDoesNotFit_ThrowsValidationException()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var learningSpace = new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f);  // Small space
        
        var request = new CreateWhiteboardRequest(
            ValidComponentId, learningSpaceId, 10.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);  // Whiteboard too large

        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync(learningSpace);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _sut.CreateWhiteboardAsync(request));
        Assert.That(ex!.Message, Does.Contain("does not fit"));
        _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify service throws DatabaseException when repository fails to save whiteboard.
    /// </summary>
    [Test]
    [Description("Application-004: Verify service throws DatabaseException when repository fails to save")]
    public void CreateWhiteboardAsync_RepositoryFails_ThrowsDatabaseException()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var learningSpace = new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);
        
        var request = new CreateWhiteboardRequest(
            ValidComponentId, learningSpaceId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepository
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .ThrowsAsync(new DatabaseException("DB error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<DatabaseException>(() => _sut.CreateWhiteboardAsync(request));
        Assert.That(ex!.Message, Does.Contain("DB error"));
    }
}
