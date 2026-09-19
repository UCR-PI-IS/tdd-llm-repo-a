using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="WhiteboardService.UpdateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-005 for CPD-LC-001-005.
/// </summary>
[TestFixture]
public class WhiteboardServiceUpdateTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepository = null!;
    private Mock<ILearningSpaceReadRepository> _mockLearningSpaceReadRepository = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidWhiteboardId = "WB-001";
    private const string ValidLearningSpaceId = "LS-001";
    private const string OtherWhiteboardId = "WB-002";

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

    private static Whiteboard CreateExistingWhiteboard()
    {
        return new Whiteboard(
            ValidWhiteboardId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");
    }

    /// <summary>
    /// Application-001: Verify that service successfully updates a whiteboard
    /// when valid data is provided.
    /// </summary>
    [Test]
    [Description("Application-001: Successfully updates whiteboard when valid data is provided")]
    public async Task UpdateWhiteboardAsync_ValidData_ReturnsSuccess()
    {
        // Arrange
        var existing = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existing);
        _mockWhiteboardRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        var dto = new UpdateWhiteboardDto(
            ValidWhiteboardId,
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "Red");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            _mockWhiteboardRepository.Verify(
                r => r.UpdateAsync(It.Is<Whiteboard>(w => w.MarkerColor == "Red")),
                Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that service returns failure when whiteboard
    /// with the given ID does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Returns failure when whiteboard with given ID does not exist")]
    public async Task UpdateWhiteboardAsync_WhiteboardNotFound_ReturnsFailure()
    {
        // Arrange
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync((Whiteboard?)null);

        var dto = new UpdateWhiteboardDto(
            ValidWhiteboardId,
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "Red");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Whiteboard not found"));
            _mockWhiteboardRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Whiteboard>()),
                Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that service returns failure when validation fails
    /// (invalid markerColor).
    /// </summary>
    [Test]
    [Description("Application-003: Returns failure when validation fails (invalid markerColor)")]
    public async Task UpdateWhiteboardAsync_InvalidMarkerColor_ReturnsFailure()
    {
        // Arrange
        var existing = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existing);

        var dto = new UpdateWhiteboardDto(
            ValidWhiteboardId,
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Invalid marker color"));
            _mockWhiteboardRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Whiteboard>()),
                Times.Never);
        });
    }

    /// <summary>
    /// Application-004: Verify that service returns failure when new position
    /// exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Application-004: Returns failure when new position exceeds learning space boundaries")]
    public async Task UpdateWhiteboardAsync_PositionExceedsBoundaries_ReturnsFailure()
    {
        // Arrange
        var existing = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existing);
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f));

        var dto = new UpdateWhiteboardDto(
            ValidWhiteboardId,
            2.0f, 1.5f, 0.1f,
            10.0f, 0.0f, 10.0f,
            "North", "Blue");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position exceeds learning space boundaries"));
            _mockWhiteboardRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Whiteboard>()),
                Times.Never);
        });
    }

    /// <summary>
    /// Application-005: Verify that service returns failure when new position
    /// overlaps with another component in the same learning space.
    /// </summary>
    [Test]
    [Description("Application-005: Returns failure when new position overlaps with another component")]
    public async Task UpdateWhiteboardAsync_PositionOverlaps_ReturnsFailure()
    {
        // Arrange
        var existing = CreateExistingWhiteboard();
        var otherComponent = new Whiteboard(
            OtherWhiteboardId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "North", "Green");

        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existing);
        _mockWhiteboardRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent> { otherComponent });
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f));

        var dto = new UpdateWhiteboardDto(
            ValidWhiteboardId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "North", "Blue");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position overlaps with existing component"));
            _mockWhiteboardRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Whiteboard>()),
                Times.Never);
        });
    }
}
