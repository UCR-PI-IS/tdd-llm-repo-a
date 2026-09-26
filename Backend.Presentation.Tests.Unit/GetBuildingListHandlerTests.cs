using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="GetBuildingListHandler.HandleAsync"/>.
/// Covers intents Presentation-003 through Presentation-004.
/// </summary>
[TestFixture]
public class GetBuildingListHandlerTests
{
    private Mock<IBuildingListService> _mockService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IBuildingListService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-003: Verify handler returns OK response with list of buildings
    /// when service returns multiple buildings.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns OK with building list when service has multiple buildings")]
    public async Task HandleAsync_MultipleBuildings_ReturnsOkWithBuildingList()
    {
        // Arrange
        var buildings = new List<Building>
        {
            new Building(1, "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f),
            new Building(2, "Science Building", "Blue", 25.0f, 60.0f, 40.0f, 150.0f, 250.0f, 0.0f)
        };

        _mockService
            .Setup(s => s.GetAllBuildingsAsync())
            .ReturnsAsync(buildings);

        // Act
        var result = await GetBuildingListHandler.HandleAsync(_mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetBuildingListResponse>>());
            var okResult = result as Ok<GetBuildingListResponse>;
            Assert.That(okResult!.Value!.Buildings, Has.Count.EqualTo(2));
            Assert.That(okResult.Value.Buildings[0].Id, Is.EqualTo(1));
            Assert.That(okResult.Value.Buildings[0].Name, Is.EqualTo("Engineering Building"));
            Assert.That(okResult.Value.Buildings[1].Id, Is.EqualTo(2));
            Assert.That(okResult.Value.Buildings[1].Name, Is.EqualTo("Science Building"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify handler returns OK response with empty list when service returns no buildings.
    /// </summary>
    [Test]
    [Description("Presentation-004: Handler returns OK with empty list when service has no buildings")]
    public async Task HandleAsync_NoBuildings_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyBuildings = new List<Building>();

        _mockService
            .Setup(s => s.GetAllBuildingsAsync())
            .ReturnsAsync(emptyBuildings);

        // Act
        var result = await GetBuildingListHandler.HandleAsync(_mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetBuildingListResponse>>());
            var okResult = result as Ok<GetBuildingListResponse>;
            Assert.That(okResult!.Value!.Buildings, Is.Not.Null);
            Assert.That(okResult.Value.Buildings, Is.Empty);
            Assert.That(okResult.Value.Buildings, Has.Count.EqualTo(0));
        });
    }
}
