using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UpdateBuildingHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003 for story PQL-AE-001-002.
/// </summary>
[TestFixture]
public class UpdateBuildingHandlerTests
{
    private Mock<IBuildingService> _mockService = null!;

    // Valid test data
    private const int ValidBuildingId = 1;
    private const string ValidName = "Updated Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 12.5f;
    private const float ValidLength = 25.0f;
    private const float ValidWidth = 18.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 5.0f;
    private const float ValidZ = 200.0f;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IBuildingService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    private static UpdateBuildingDto CreateValidUpdateDto()
    {
        return new UpdateBuildingDto(
            ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);
    }

    /// <summary>
    /// Presentation-001: Verify that updating a building with valid data returns 200 OK
    /// with a success response.
    /// </summary>
    [Test]
    [Description("Presentation-001: Updating a building with valid data returns 200 OK")]
    public async Task HandleAsync_ValidData_ReturnsOk()
    {
        // Arrange
        var updateDto = CreateValidUpdateDto();

        _mockService
            .Setup(s => s.UpdateBuildingAsync(ValidBuildingId, It.IsAny<UpdateBuildingDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(ValidBuildingId, updateDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Ok<UpdateBuildingResponse>>());
    }

    /// <summary>
    /// Presentation-002: Verify that updating a building with invalid data returns 400 Bad Request
    /// with an error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Updating a building with invalid data returns 400 Bad Request")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidDto = new UpdateBuildingDto(
            "", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(ValidBuildingId, It.IsAny<UpdateBuildingDto>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty", "name"));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(ValidBuildingId, invalidDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
    }

    /// <summary>
    /// Presentation-003: Verify that updating a non-existing building returns 404 Not Found
    /// with an error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Updating a non-existing building returns 404 Not Found")]
    public async Task HandleAsync_BuildingNotFound_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = 999;
        var updateDto = CreateValidUpdateDto();

        _mockService
            .Setup(s => s.UpdateBuildingAsync(nonExistentId, It.IsAny<UpdateBuildingDto>()))
            .ThrowsAsync(new BuildingNotFoundException(nonExistentId));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(nonExistentId, updateDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFound<string>>());
    }
}
