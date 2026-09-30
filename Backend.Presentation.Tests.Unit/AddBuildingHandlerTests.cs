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
/// Unit tests for <see cref="AddBuildingHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003.
/// </summary>
[TestFixture]
public class AddBuildingHandlerTests
{
    private Mock<IBuildingService> _mockService = null!;

    // Valid test data
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Red";
    private const float ValidHeight = 20.5f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;
    private const int ValidAreaId = 1;

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

    private static CreateBuildingDto CreateValidDto()
    {
        return new CreateBuildingDto(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);
    }

    private static Building CreateValidBuilding()
    {
        return new Building(1, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);
    }

    /// <summary>
    /// Presentation-001: Verify that a valid building creation request returns 201 Created with the building data.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 201 Created with building data when request is valid")]
    public async Task HandleAsync_ValidRequest_ReturnsCreatedWithBuildingData()
    {
        // Arrange
        var building = CreateValidBuilding();
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>(), ValidAreaId))
            .ReturnsAsync(building);

        var dto = CreateValidDto();

        // Act
        var result = await AddBuildingHandler.HandleAsync(dto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Created<AddBuildingResponse>>());
            var createdResult = (Created<AddBuildingResponse>)result.Result;
            Assert.That(createdResult.Value.Building.Name, Is.EqualTo(ValidName));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that attempting to add a duplicate building returns 409 Conflict with error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 409 Conflict when building already exists")]
    public async Task HandleAsync_DuplicateBuilding_ReturnsConflict()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>(), ValidAreaId))
            .ThrowsAsync(new DuplicateBuildingException($"Building with name '{ValidName}' already exists"));

        var dto = CreateValidDto();

        // Act
        var result = await AddBuildingHandler.HandleAsync(dto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<string>>());
            var conflictResult = (Conflict<string>)result.Result;
            Assert.That(conflictResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that attempting to add a building with invalid data returns 400 Bad Request with error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 400 Bad Request when input validation fails")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>(), ValidAreaId))
            .ThrowsAsync(new ArgumentException("Name cannot be empty"));

        var dto = CreateValidDto();

        // Act
        var result = await AddBuildingHandler.HandleAsync(dto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("cannot be empty"));
        });
    }
}
