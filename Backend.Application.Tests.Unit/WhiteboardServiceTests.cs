using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
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
    /// Application-001: Verify service successfully creates and returns a whiteboard
    /// when it fits in the learning space.
    /// </summary>
    [Test]
    [Description("Application-001: Successfully create whiteboard when it fits in learning space")]
    public async Task CreateWhiteboardAsync_ValidRequestAndFits_ReturnsWhiteboard()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            "LS-001", "WB-001", 2.0f, 1.5f, 0.5f, 1.0f, 1.0f, 0.0f, "North", "Blue");
        var learningSpace = new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f);

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync("LS-001"))
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
            Assert.That(result.LearningSpaceId, Is.EqualTo("LS-001"));
            Assert.That(result.MarkerColor, Is.EqualTo("Blue"));
            _mockWhiteboardRepo.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify service throws NotFoundException when the learning space
    /// does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Throw exception when learning space does not exist")]
    public async Task CreateWhiteboardAsync_LearningSpaceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            "LS-001", "WB-001", 2.0f, 1.5f, 0.5f, 1.0f, 1.0f, 0.0f, "North", "Blue");

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync("LS-001"))
            .ReturnsAsync((LearningSpace?)null);

        // Act
        NotFoundException? caughtException = null;
        try
        {
            await _sut.CreateWhiteboardAsync(request);
        }
        catch (NotFoundException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected NotFoundException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Learning space not found"));
        });
    }

    /// <summary>
    /// Application-003: Verify service throws ValidationException when the whiteboard
    /// does not fit in the learning space.
    /// </summary>
    [Test]
    [Description("Application-003: Throw exception when whiteboard doesn't fit in learning space")]
    public async Task CreateWhiteboardAsync_WhiteboardDoesNotFit_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            "LS-001", "WB-001", 10.0f, 10.0f, 10.0f, 0.0f, 0.0f, 0.0f, "North", "Blue");
        var learningSpace = new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f);

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync("LS-001"))
            .ReturnsAsync(learningSpace);

        // Act
        ValidationException? caughtException = null;
        try
        {
            await _sut.CreateWhiteboardAsync(request);
        }
        catch (ValidationException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ValidationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("does not fit"));
            _mockWhiteboardRepo.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-004: Verify service throws DatabaseException when the repository
    /// fails to save the whiteboard.
    /// </summary>
    [Test]
    [Description("Application-004: Throw exception when repository fails to save whiteboard")]
    public async Task CreateWhiteboardAsync_RepositorySaveFails_ThrowsDatabaseException()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            "LS-001", "WB-001", 2.0f, 1.5f, 0.5f, 1.0f, 1.0f, 0.0f, "North", "Blue");
        var learningSpace = new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f);

        _mockLearningSpaceRepo
            .Setup(r => r.GetByIdAsync("LS-001"))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepo
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .ThrowsAsync(new DatabaseException("DB error"));

        // Act
        DatabaseException? caughtException = null;
        try
        {
            await _sut.CreateWhiteboardAsync(request);
        }
        catch (DatabaseException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DatabaseException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("DB error"));
        });
    }
}
