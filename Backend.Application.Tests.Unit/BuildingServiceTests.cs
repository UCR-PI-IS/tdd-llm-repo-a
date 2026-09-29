using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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

    // Valid test data
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 15.0f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;
    private const int ValidAreaId = 1;

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

    private Building CreateValidBuilding()
    {
        return new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
    }

    /// <summary>
    /// Application-001: Verify that a valid building is successfully added when no duplicate exists.
    /// </summary>
    [Test]
    [Description("Application-001: Valid building is successfully added when no duplicate exists")]
    public async Task AddBuildingAsync_NoDuplicate_ReturnsBuilding()
    {
        // Arrange
        var building = CreateValidBuilding();

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AreaExistsAsync(ValidAreaId))
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
            Assert.That(result.Name, Is.EqualTo(building.Name));
            _mockRepository.Verify(r => r.AddAsync(building), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that adding a building with duplicate name throws DuplicateBuildingException.
    /// </summary>
    [Test]
    [Description("Application-002: Adding building with duplicate name throws DuplicateBuildingException")]
    public void AddBuildingAsync_DuplicateName_ThrowsDuplicateBuildingException()
    {
        // Arrange
        var building = CreateValidBuilding();

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateBuildingException>(() =>
            _sut.AddBuildingAsync(building));

        Assert.That(ex.Message, Does.Contain($"Building with name '{ValidName}' already exists"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a building to a non-existent area throws AreaNotFoundException.
    /// </summary>
    [Test]
    [Description("Application-003: Adding building to non-existent area throws AreaNotFoundException")]
    public void AddBuildingAsync_NonExistentArea_ThrowsAreaNotFoundException()
    {
        // Arrange
        var building = CreateValidBuilding();

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AreaExistsAsync(ValidAreaId))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<AreaNotFoundException>(() =>
            _sut.AddBuildingAsync(building));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }
}
