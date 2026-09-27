using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingListService.GetAllBuildingsAsync"/>.
/// Covers intents Application-002 and Application-003.
/// </summary>
[TestFixture]
public class BuildingListServiceTests
{
    private Mock<IBuildingListRepository> _mockRepository = null!;
    private BuildingListService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IBuildingListRepository>();
        _sut = new BuildingListService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-002: Verify service returns all buildings from repository when buildings exist.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service returns all buildings from repository when buildings exist")]
    public async Task GetAllBuildingsAsync_BuildingsExist_ReturnsAllBuildings()
    {
        // Arrange
        var expectedBuildings = new List<Building>
        {
            new Building(1, "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f),
            new Building(2, "Science Building", "Blue", 25.0f, 60.0f, 40.0f, 150.0f, 250.0f, 0.0f)
        };
        _mockRepository
            .Setup(r => r.GetAllBuildingsAsync())
            .ReturnsAsync(expectedBuildings);

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expectedBuildings));
            Assert.That(result, Has.Count.EqualTo(2));
            _mockRepository.Verify(r => r.GetAllBuildingsAsync(), Times.Once);
        });
    }

    /// <summary>
    /// Application-003: Verify service returns empty list when no buildings exist in repository.
    /// </summary>
    [Test]
    [Description("Application-003: Verify service returns empty list when no buildings exist in repository")]
    public async Task GetAllBuildingsAsync_NoBuildings_ReturnsEmptyList()
    {
        // Arrange
        var emptyBuildings = new List<Building>();
        _mockRepository
            .Setup(r => r.GetAllBuildingsAsync())
            .ReturnsAsync(emptyBuildings);

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Has.Count.EqualTo(0));
            _mockRepository.Verify(r => r.GetAllBuildingsAsync(), Times.Once);
        });
    }
}
