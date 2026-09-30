using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.AddBuildingAsync"/>.
/// Covers intents Application-001 through Application-003 for story PQL-AE-001-001.
/// </summary>
[TestFixture]
public class BuildingServiceTests
{
    private Mock<IBuildingRepository> _mockRepository = null!;
    private BuildingService _sut = null!;

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
        _mockRepository = new Mock<IBuildingRepository>();
        _sut = new BuildingService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that a valid building is successfully added when no duplicate exists
    /// and the area is valid. The repository AddAsync is called exactly once.
    /// </summary>
    [Test]
    [Description("Application-001: Valid building is successfully added when no duplicate exists")]
    public async Task AddBuildingAsync_ValidBuildingNoDuplicate_ReturnsAddedBuilding()
    {
        // Arrange
        var building = new Building(BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(BuildingName))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AreaExistsAsync(BuildingAreaId))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.AddAsync(building))
            .ReturnsAsync(building);

        // Act
        var result = await _sut.AddBuildingAsync(building);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(BuildingName));
        });
        _mockRepository.Verify(r => r.AddAsync(building), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a building with a duplicate name throws
    /// DuplicateBuildingException and the repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate building name throws DuplicateBuildingException")]
    public async Task AddBuildingAsync_DuplicateName_ThrowsDuplicateBuildingException()
    {
        // Arrange
        var building = new Building(BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(BuildingName))
            .ReturnsAsync(true);

        // Act
        DuplicateBuildingException? caughtException = null;
        try
        {
            await _sut.AddBuildingAsync(building);
        }
        catch (DuplicateBuildingException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DuplicateBuildingException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain($"Building with name '{BuildingName}' already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a building to a non-existent area throws
    /// AreaNotFoundException and the repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-003: Non-existent area throws AreaNotFoundException")]
    public async Task AddBuildingAsync_NonExistentArea_ThrowsAreaNotFoundException()
    {
        // Arrange
        var building = new Building(BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(BuildingName))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AreaExistsAsync(BuildingAreaId))
            .ReturnsAsync(false);

        // Act
        AreaNotFoundException? caughtException = null;
        try
        {
            await _sut.AddBuildingAsync(building);
        }
        catch (AreaNotFoundException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.That(caughtException, Is.Not.Null, "Expected AreaNotFoundException was not thrown");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }
}
