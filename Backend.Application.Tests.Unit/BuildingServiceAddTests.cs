using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.AddBuildingAsync"/>.
/// Covers intents Application-001 through Application-003.
/// </summary>
[TestFixture]
public class BuildingServiceAddTests
{
    private Mock<IBuildingRepository> _mockRepository = null!;
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
        _sut = new BuildingService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that a valid building is successfully added
    /// when no duplicate exists and the area is valid. The repository persists
    /// the entity and the service returns the created building.
    /// </summary>
    [Test]
    [Description("Application-001: Valid building is added successfully when no duplicate exists")]
    public async Task AddBuildingAsync_ValidBuildingNoDuplicate_ReturnsBuildingAndPersists()
    {
        // Arrange
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockRepository
            .Setup(r => r.AreaExistsAsync(ValidAreaId))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(building))
            .ReturnsAsync(building);

        // Act
        var result = await _sut.AddBuildingAsync(building);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(ValidName));
        });
        _mockRepository.Verify(r => r.AddAsync(building), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a building with a duplicate name
    /// throws DuplicateBuildingException and the repository never persists.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate building name throws DuplicateBuildingException")]
    public async Task AddBuildingAsync_DuplicateName_ThrowsDuplicateBuildingException()
    {
        // Arrange
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockRepository
            .Setup(r => r.AreaExistsAsync(ValidAreaId))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
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
            Assert.That(caughtException!.Message, Does.Contain("already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a building to a non-existent area
    /// throws AreaNotFoundException and the repository never persists.
    /// </summary>
    [Test]
    [Description("Application-003: Non-existent area throws AreaNotFoundException")]
    public async Task AddBuildingAsync_NonExistentArea_ThrowsAreaNotFoundException()
    {
        // Arrange
        var nonExistentAreaId = 999;
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, nonExistentAreaId);

        _mockRepository
            .Setup(r => r.AreaExistsAsync(nonExistentAreaId))
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
