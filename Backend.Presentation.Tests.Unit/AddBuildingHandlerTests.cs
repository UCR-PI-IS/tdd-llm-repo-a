using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="AddBuildingHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003 for story PQL-AE-001-001.
/// </summary>
[TestFixture]
public class AddBuildingHandlerTests
{
    private Mock<IBuildingService> _mockService = null!;

    private const string BuildingName = "Engineering Building";
    private const string BuildingColor = "Red";
    private const float BuildingHeight = 20.5f;
    private const float BuildingLength = 50.0f;
    private const float BuildingWidth = 30.0f;
    private const float BuildingX = 100.0f;
    private const float BuildingY = 200.0f;
    private const float BuildingZ = 0.0f;
    private const int BuildingAreaId = 1;

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
    /// Presentation-001: Verify that a valid building creation request returns 201 Created
    /// with the building data in the response.
    /// </summary>
    [Test]
    [Description("Presentation-001: Valid building creation request returns 201 Created with building data")]
    public async Task HandleAsync_ValidRequest_ReturnsCreatedWithBuildingData()
    {
        // Arrange
        var building = new Building(BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ReturnsAsync(building);

        var request = new AddBuildingRequest(
            BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        // Act
        var result = await AddBuildingHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Created<AddBuildingResponse>>());
            var createdResult = (Created<AddBuildingResponse>)result;
            Assert.That(createdResult.Value!.Building.Name, Is.EqualTo(BuildingName));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that attempting to add a duplicate building returns 409 Conflict
    /// with an appropriate error message containing "already exists".
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate building returns 409 Conflict with error message")]
    public async Task HandleAsync_DuplicateBuilding_ReturnsConflict()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new DuplicateBuildingException($"Building with name '{BuildingName}' already exists"));

        var request = new AddBuildingRequest(
            BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        // Act
        var result = await AddBuildingHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Conflict<string>>());
            var conflictResult = (Conflict<string>)result;
            Assert.That(conflictResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that attempting to add a building with invalid data returns
    /// 400 Bad Request with an appropriate error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid building data returns 400 Bad Request with error message")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty"));

        var request = new AddBuildingRequest(
            "", BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        // Act
        var result = await AddBuildingHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result;
            Assert.That(badRequestResult.Value, Does.Contain("cannot be empty"));
        });
    }
}
