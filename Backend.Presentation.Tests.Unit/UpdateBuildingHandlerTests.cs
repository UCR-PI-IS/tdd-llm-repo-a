using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

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
    private const int ValidInternalId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
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

    /// <summary>
    /// Presentation-001: Verify that updating a building with valid data returns
    /// 200 OK with an <see cref="UpdateBuildingResponse"/> containing the updated building.
    /// </summary>
    [Test]
    [Description("Presentation-001: Valid building update returns 200 OK with success response")]
    public async Task HandleAsync_ValidData_ReturnsOkWithUpdateBuildingResponse()
    {
        // Arrange
        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(ValidInternalId, It.IsAny<UpdateBuildingDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(ValidInternalId, updateDto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<UpdateBuildingResponse>>());
            var okResult = (Ok<UpdateBuildingResponse>)result;
            Assert.That(okResult.Value, Is.Not.Null);
        });
    }

    /// <summary>
    /// Presentation-002: Verify that updating a building with invalid data returns
    /// 400 Bad Request with an error message describing the validation failure.
    /// </summary>
    [Test]
    [Description("Presentation-002: Invalid building update data returns 400 Bad Request with error message")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequestWithErrorMessage()
    {
        // Arrange
        var invalidDto = new UpdateBuildingDto(
            "", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(ValidInternalId, It.IsAny<UpdateBuildingDto>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty", "name"));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(ValidInternalId, invalidDto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result;
            Assert.That(badRequestResult.Value, Does.Contain("cannot be empty"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that updating a non-existing building returns
    /// 404 Not Found with an error message indicating the building was not found.
    /// </summary>
    [Test]
    [Description("Presentation-003: Update of non-existing building returns 404 Not Found")]
    public async Task HandleAsync_NonExistingBuilding_ReturnsNotFoundWithErrorMessage()
    {
        // Arrange
        var nonExistentId = 999;
        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(nonExistentId, It.IsAny<UpdateBuildingDto>()))
            .ThrowsAsync(new BuildingNotFoundException(nonExistentId));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(nonExistentId, updateDto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<NotFound<string>>());
            var notFoundResult = (NotFound<string>)result;
            Assert.That(notFoundResult.Value, Does.Contain("not found"));
        });
    }
}
