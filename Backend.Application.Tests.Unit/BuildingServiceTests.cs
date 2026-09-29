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
    private Mock<IAreaRepository> _mockAreaRepository = null!;
    private BuildingService _sut = null!;

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
        _mockRepository = new Mock<IBuildingRepository>();
        _mockAreaRepository = new Mock<IAreaRepository>();
        _sut = new BuildingService(_mockRepository.Object, _mockAreaRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
        _mockAreaRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that a valid building is successfully added when no duplicate exists.
    /// </summary>
    [Test]
    [Description("Application-001: Valid building added successfully when no duplicate exists")]
    public async Task AddBuildingAsync_ValidBuildingNoDuplicate_ReturnsSuccessAndPersists()
    {
        // Arrange
        var building = new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
        
        _mockAreaRepository
            .Setup(r => r.ExistsAsync(ValidAreaId))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddBuildingAsync(building);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(building.Name));
        _mockRepository.Verify(r => r.AddAsync(building), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a building with duplicate name throws DuplicateBuildingException.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate building name throws DuplicateBuildingException")]
    public void AddBuildingAsync_DuplicateName_ThrowsDuplicateBuildingException()
    {
        // Arrange
        var building = new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
        
        _mockAreaRepository
            .Setup(r => r.ExistsAsync(ValidAreaId))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DuplicateBuildingException>(() => _sut.AddBuildingAsync(building));
        Assert.That(ex.Message, Does.Contain("Building with name 'Engineering Building' already exists"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a building to a non-existent area throws AreaNotFoundException.
    /// </summary>
    [Test]
    [Description("Application-003: Non-existent area throws AreaNotFoundException")]
    public void AddBuildingAsync_NonExistentArea_ThrowsAreaNotFoundException()
    {
        // Arrange
        var building = new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
        
        _mockAreaRepository
            .Setup(r => r.ExistsAsync(ValidAreaId))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<AreaNotFoundException>(() => _sut.AddBuildingAsync(building));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }
}
