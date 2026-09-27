using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingListService.GetAllBuildingsAsync"/>.
/// Covers intents Application-001 through Application-003.
/// </summary>
[TestFixture]
public class BuildingListServiceTests
{
    private Mock<IBuildingListRepository> _mockRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IBuildingListRepository>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify service interface defines GetAllBuildingsAsync method that returns Task&lt;List&lt;Building&gt;&gt;.
    /// </summary>
    [Test]
    [Description("Application-001: IBuildingListService interface defines GetAllBuildingsAsync method")]
    public void IBuildingListService_InterfaceContract_DefinesGetAllBuildingsAsync()
    {
        // Arrange & Act
        var method = typeof(IBuildingListService).GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null);
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)));
            Assert.That(method.GetParameters(), Is.Empty);
        });
    }

    /// <summary>
    /// Application-002: Verify service returns all buildings from repository when buildings exist.
    /// </summary>
    [Test]
    [Description("Application-002: Service returns all buildings from repository when buildings exist")]
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

        var service = new BuildingListService(_mockRepository.Object);

        // Act
        var result = await service.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expectedBuildings));
            Assert.That(result.Count, Is.EqualTo(2));
        });
        _mockRepository.Verify(r => r.GetAllBuildingsAsync(), Times.Once);
    }

    /// <summary>
    /// Application-003: Verify service returns empty list when no buildings exist in repository.
    /// </summary>
    [Test]
    [Description("Application-003: Service returns empty list when no buildings exist")]
    public async Task GetAllBuildingsAsync_NoBuildingsExist_ReturnsEmptyList()
    {
        // Arrange
        var emptyBuildings = new List<Building>();

        _mockRepository
            .Setup(r => r.GetAllBuildingsAsync())
            .ReturnsAsync(emptyBuildings);

        var service = new BuildingListService(_mockRepository.Object);

        // Act
        var result = await service.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result.Count, Is.EqualTo(0));
        });
        _mockRepository.Verify(r => r.GetAllBuildingsAsync(), Times.Once);
    }
}
