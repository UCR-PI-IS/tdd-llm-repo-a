using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.AddBuildingAsync"/>.
/// Covers intents Application-001 through Application-003.
/// </summary>
[TestFixture]
public class BuildingServiceTests
{
    private Mock<IBuildingRepository> _mockRepository = null!;
    private BuildingService _sut = null!;

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
    /// Application-001: Verify that a valid building is successfully added when no duplicate exists.
    /// </summary>
    [Test]
    [Description("Application-001: Valid building is successfully added when no duplicate exists")]
    public async Task AddBuildingAsync_ValidBuildingNoDuplicate_ReturnsAddedBuilding()
    {
        // Arrange
        var building = new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);
        _mockRepository.Setup(r => r.ExistsByNameAsync(building.Name)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AreaExistsAsync(building.AreaId)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.AddAsync(building)).ReturnsAsync(building);

        // Act
        var result = await _sut.AddBuildingAsync(building);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(building.Name));
        });
        _mockRepository.Verify(r => r.AddAsync(building), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a building with duplicate name throws DuplicateBuildingException.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate building name throws DuplicateBuildingException")]
    public async Task AddBuildingAsync_DuplicateName_ThrowsDuplicateBuildingException()
    {
        // Arrange
        var building = new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);
        _mockRepository.Setup(r => r.ExistsByNameAsync(building.Name)).ReturnsAsync(true);

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
            Assert.That(caughtException, Is.Not.Null);
            Assert.That(caughtException!.Message, Does.Contain("already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a building to a non-existent area throws AreaNotFoundException.
    /// </summary>
    [Test]
    [Description("Application-003: Non-existent area throws AreaNotFoundException")]
    public async Task AddBuildingAsync_NonExistentArea_ThrowsAreaNotFoundException()
    {
        // Arrange
        var building = new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);
        _mockRepository.Setup(r => r.ExistsByNameAsync(building.Name)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AreaExistsAsync(building.AreaId)).ReturnsAsync(false);

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
        Assert.That(caughtException, Is.Not.Null);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }
}
