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
    private Mock<ILearningSpaceReadRepository> _mockLearningSpaceRepository = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 1.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceRepository = new Mock<ILearningSpaceReadRepository>();
        _sut = new WhiteboardService(_mockWhiteboardRepository.Object, _mockLearningSpaceRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepository.VerifyAll();
        _mockLearningSpaceRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that the service successfully creates a whiteboard
    /// when it fits in the learning space and returns the created entity.
    /// </summary>
    [Test]
    [Description("Application-001: Service creates whiteboard when it fits in learning space")]
    public async Task CreateWhiteboardAsync_ValidInput_ReturnsCreatedWhiteboard()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 12.0f);
        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepository
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

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
        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync((LearningSpace?)null);

        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

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
    /// when the whiteboard doesn't fit in the learning space.
    /// </summary>
    [Test]
    [Description("Application-003: Service throws ValidationException when whiteboard doesn't fit")]
    public async Task CreateWhiteboardAsync_WhiteboardDoesNotFit_ThrowsValidationException()
    {
        // Arrange
        var smallSpace = new LearningSpace("Classroom", 2.0f, 3.0f, 3.0f);
        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(smallSpace);

        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId,
            5.0f, 3.0f, 1.0f,  // Whiteboard too large for the space
            0.0f, 0.0f, 0.0f,
            ValidOrientation, ValidMarkerColor);

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
        });
        _mockWhiteboardRepository.Verify(r => r.AddAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that the service throws DatabaseException
    /// when the repository fails to save the whiteboard.
    /// </summary>
    [Test]
    [Description("Application-004: Service throws DatabaseException when repository fails")]
    public async Task CreateWhiteboardAsync_RepositoryFails_ThrowsDatabaseException()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 12.0f);
        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(learningSpace);

        _mockWhiteboardRepository
            .Setup(r => r.AddAsync(It.IsAny<Whiteboard>()))
            .ThrowsAsync(new DatabaseException("DB error"));

        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

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
