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
/// Unit tests for <see cref="WhiteboardService.CreateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-004.
/// </summary>
[TestFixture]
public class WhiteboardServiceTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepository = null!;
    private Mock<ILearningSpaceReadRepository> _mockLearningSpaceReadRepository = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.5f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    // Learning space dimensions that accommodate the valid whiteboard
    private const float SpaceHeight = 3.0f;
    private const float SpaceWidth = 8.0f;
    private const float SpaceLength = 10.0f;

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceReadRepository = new Mock<ILearningSpaceReadRepository>();
        _sut = new WhiteboardService(
            _mockWhiteboardRepository.Object,
            _mockLearningSpaceReadRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepository.VerifyAll();
        _mockLearningSpaceReadRepository.VerifyAll();
    }

    private static CreateWhiteboardRequest CreateValidRequest(
        string? learningSpaceId = null,
        float? width = null,
        float? height = null,
        float? depth = null,
        float? x = null,
        float? y = null,
        float? z = null)
    {
        return new CreateWhiteboardRequest(
            ValidComponentId,
            learningSpaceId ?? ValidLearningSpaceId,
            width ?? ValidWidth,
            height ?? ValidHeight,
            depth ?? ValidDepth,
            x ?? ValidX,
            y ?? ValidY,
            z ?? ValidZ,
            ValidOrientation,
            ValidMarkerColor);
    }

    private static LearningSpace CreateLearningSpace(
        float? height = null,
        float? width = null,
        float? length = null)
    {
        return new LearningSpace(
            "Classroom",
            height ?? SpaceHeight,
            width ?? SpaceWidth,
            length ?? SpaceLength);
    }

    /// <summary>
    /// Application-001: Verify that the service successfully creates a whiteboard
    /// when it fits in the learning space and persists it via the repository.
    /// </summary>
    [Test]
    [Description("Application-001: Service creates whiteboard with valid input when it fits in learning space")]
    public async Task CreateWhiteboardAsync_ValidInputAndFits_ReturnsCreatedWhiteboard()
    {
        // Arrange
        var request = CreateValidRequest();
        var learningSpace = CreateLearningSpace();

        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
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
            Assert.That(result.LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
            Assert.That(result.MarkerColor, Is.EqualTo(ValidMarkerColor));
        });
        _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that the service throws NotFoundException
    /// when the learning space does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Service throws NotFoundException when learning space does not exist")]
    public async Task CreateWhiteboardAsync_LearningSpaceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var request = CreateValidRequest();

        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
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
        _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that the service throws ValidationException
    /// when the whiteboard doesn't fit in the learning space and does not call the repository.
    /// </summary>
    [Test]
    [Description("Application-003: Service throws ValidationException when whiteboard does not fit in learning space")]
    public async Task CreateWhiteboardAsync_WhiteboardDoesNotFit_ThrowsValidationException()
    {
        // Arrange
        var oversizedRequest = CreateValidRequest(width: 20f, x: 0f);
        var learningSpace = CreateLearningSpace();

        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(learningSpace);

        // Act
        ValidationException? caughtException = null;
        try
        {
            await _sut.CreateWhiteboardAsync(oversizedRequest);
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
        });
        _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that the service propagates DatabaseException
    /// when the repository fails to save the whiteboard.
    /// </summary>
    [Test]
    [Description("Application-004: Service propagates DatabaseException when repository fails to save")]
    public async Task CreateWhiteboardAsync_RepositoryFails_ThrowsDatabaseException()
    {
        // Arrange
        var request = CreateValidRequest();
        var learningSpace = CreateLearningSpace();

        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepository
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
