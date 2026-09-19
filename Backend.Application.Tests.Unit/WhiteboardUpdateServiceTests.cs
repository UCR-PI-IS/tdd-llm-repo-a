using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="WhiteboardService.UpdateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-005 for CPD-LC-001-005.
/// </summary>
[TestFixture]
public class WhiteboardUpdateServiceTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepository = null!;
    private Mock<ILearningSpaceReadRepository> _mockLearningSpaceReadRepository = null!;
    private Mock<ILearningComponentRepository> _mockLearningComponentRepository = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidWhiteboardId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceReadRepository = new Mock<ILearningSpaceReadRepository>();
        _mockLearningComponentRepository = new Mock<ILearningComponentRepository>();
        _sut = new WhiteboardService(
            _mockWhiteboardRepository.Object,
            _mockLearningSpaceReadRepository.Object,
            _mockLearningComponentRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepository.VerifyAll();
        _mockLearningSpaceReadRepository.VerifyAll();
        _mockLearningComponentRepository.VerifyAll();
    }

    private static UpdateWhiteboardRequest CreateValidUpdateRequest(
        string whiteboardId = ValidWhiteboardId,
        float width = ValidWidth,
        float height = ValidHeight,
        float depth = ValidDepth,
        float x = ValidX,
        float y = ValidY,
        float z = ValidZ,
        string orientation = ValidOrientation,
        string markerColor = ValidMarkerColor)
    {
        return new UpdateWhiteboardRequest(
            whiteboardId, width, height, depth, x, y, z, orientation, markerColor);
    }

    private static Whiteboard CreateExistingWhiteboard()
    {
        return new Whiteboard(
            ValidWhiteboardId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");
    }

    private static LearningSpace CreateLearningSpaceThatFits()
    {
        return new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);
    }

    /// <summary>
    /// Application-001: Verify that service successfully updates a whiteboard when valid data is provided.
    /// </summary>
    [Test]
    [Description("Application-001: Service successfully updates whiteboard with valid data")]
    public async Task UpdateWhiteboardAsync_ValidData_ReturnsSuccess()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        var learningSpace = CreateLearningSpaceThatFits();

        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(learningSpace);
        _mockLearningComponentRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());
        _mockWhiteboardRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        var request = CreateValidUpdateRequest(markerColor: "Red");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.MarkerColor, Is.EqualTo("Red"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.Is<Whiteboard>(w => w.MarkerColor == "Red")), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that service returns failure when whiteboard with given ID does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Service returns failure when whiteboard not found")]
    public async Task UpdateWhiteboardAsync_WhiteboardNotFound_ReturnsFailure()
    {
        // Arrange
        var nonExistentWhiteboardId = "WB-NOTFOUND";
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(nonExistentWhiteboardId))
            .ReturnsAsync((Whiteboard?)null);

        var request = CreateValidUpdateRequest(whiteboardId: nonExistentWhiteboardId);

        // Act
        var result = await _sut.UpdateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Whiteboard not found"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that service returns failure when validation fails (invalid markerColor).
    /// </summary>
    [Test]
    [Description("Application-003: Service returns failure when validation fails for invalid markerColor")]
    public async Task UpdateWhiteboardAsync_InvalidMarkerColor_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);

        var request = CreateValidUpdateRequest(markerColor: "");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Invalid marker color"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that service returns failure when new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Application-004: Service returns failure when position exceeds learning space boundaries")]
    public async Task UpdateWhiteboardAsync_PositionExceedsBoundaries_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        var smallSpace = new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f);

        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(smallSpace);

        var request = CreateValidUpdateRequest(x: 10.0f, z: 10.0f);

        // Act
        var result = await _sut.UpdateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position exceeds learning space boundaries"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that service returns failure when new position overlaps with another component in the same learning space.
    /// </summary>
    [Test]
    [Description("Application-005: Service returns failure when position overlaps with another component")]
    public async Task UpdateWhiteboardAsync_PositionOverlaps_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        var learningSpace = new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f);
        var otherComponent = new Whiteboard(
            "WB-002", ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "North", "Green");

        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(learningSpace);
        _mockLearningComponentRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent> { otherComponent });

        var request = CreateValidUpdateRequest(x: 5.0f, z: 5.0f);

        // Act
        var result = await _sut.UpdateWhiteboardAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position overlaps with existing component"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }
}

/// <summary>
/// Request object for updating a whiteboard.
/// </summary>
public record class UpdateWhiteboardRequest(
    string WhiteboardId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);

/// <summary>
/// Result object for whiteboard update operations.
/// </summary>
public class UpdateWhiteboardResult
{
    public bool IsSuccess { get; private set; }
    public Whiteboard? Value { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static UpdateWhiteboardResult Success(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResult { IsSuccess = true, Value = whiteboard };
    }

    public static UpdateWhiteboardResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardResult { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
