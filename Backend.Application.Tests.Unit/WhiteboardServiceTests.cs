using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="WhiteboardService.CreateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-004.
/// </summary>
[TestFixture]
public class WhiteboardServiceTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepo = null!;
    private Mock<ILearningSpaceRepository> _mockLearningSpaceRepo = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";
    private const string ValidComponentId = "WB-001";

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepo = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceRepo = new Mock<ILearningSpaceRepository>();
        _sut = new WhiteboardService(_mockWhiteboardRepo.Object, _mockLearningSpaceRepo.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepo.VerifyAll();
        _mockLearningSpaceRepo.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify service successfully creates whiteboard when it fits in learning space.
    /// </summary>
    [Test]
    [Description("Application-001: Successfully create whiteboard when it fits in learning space")]
    public async Task CreateWhiteboardAsync_WhiteboardFits_ReturnsCreatedWhiteboard()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var learningSpace = new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);

        var request = new CreateWhiteboardRequest(
            learningSpaceId, ValidComponentId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepo
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(result.MarkerColor, Is.EqualTo("Blue"));
        });
        _mockWhiteboardRepo.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify service throws NotFoundException when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Throw exception when learning space does not exist")]
    public void CreateWhiteboardAsync_LearningSpaceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var learningSpaceId = "NON-EXISTENT";

        var request = new CreateWhiteboardRequest(
            learningSpaceId, ValidComponentId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync((LearningSpace?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _sut.CreateWhiteboardAsync(request));
        Assert.That(ex.Message, Does.Contain("Learning space not found"));
    }

    /// <summary>
    /// Application-003: Verify service throws ValidationException when whiteboard doesn't fit in learning space.
    /// </summary>
    [Test]
    [Description("Application-003: Throw exception when whiteboard doesn't fit in learning space")]
    public void CreateWhiteboardAsync_WhiteboardDoesNotFit_ThrowsValidationException()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var learningSpace = new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);

        // Whiteboard that doesn't fit (too wide)
        var request = new CreateWhiteboardRequest(
            learningSpaceId, ValidComponentId, 15.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync(learningSpace);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(async () =>
            await _sut.CreateWhiteboardAsync(request));
        Assert.That(ex.Message, Does.Contain("does not fit"));
        _mockWhiteboardRepo.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify service throws DatabaseException when repository fails to save whiteboard.
    /// </summary>
    [Test]
    [Description("Application-004: Throw exception when repository fails to save whiteboard")]
    public void CreateWhiteboardAsync_RepositoryFails_ThrowsDatabaseException()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var learningSpace = new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);

        var request = new CreateWhiteboardRequest(
            learningSpaceId, ValidComponentId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync(learningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepo
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .ThrowsAsync(new DatabaseException("DB error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<DatabaseException>(async () =>
            await _sut.CreateWhiteboardAsync(request));
        Assert.That(ex.Message, Does.Contain("DB error"));
    }
}
