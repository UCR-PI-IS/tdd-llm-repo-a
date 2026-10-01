using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.UpdateBuildingAsync"/>.
/// Covers intents Application-001 through Application-003 for story PQL-AE-001-002 (Edit Building).
/// </summary>
[TestFixture]
public class BuildingServiceUpdateTests
{
    private Mock<IBuildingRepository> _mockRepository = null!;
    private BuildingService _sut = null!;

    // Valid test data
    private const int ValidBuildingId = 1;
    private const string ValidName = "New Name";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 12.5f;
    private const float ValidLength = 25.0f;
    private const float ValidWidth = 18.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 5.0f;
    private const float ValidZ = 200.0f;

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
    /// Creates a pre-existing building entity for update tests.
    /// </summary>
    private static Building CreateExistingBuilding()
    {
        return new Building(1, "Old Name", "Red", 10.0f, 20.0f, 15.0f, 0f, 0f, 0f);
    }

    /// <summary>
    /// Creates a valid update DTO for successful update tests.
    /// </summary>
    private static UpdateBuildingDto CreateValidUpdateDto()
    {
        return new UpdateBuildingDto(
            ValidName, ValidColor, ValidHeight, ValidLength,
            ValidWidth, ValidX, ValidY, ValidZ);
    }

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data succeeds.
    /// The service should retrieve the building, apply the update, and persist via the repository.
    /// </summary>
    [Test]
    [Description("Application-001: UpdateBuildingAsync with valid data updates building and calls repository")]
    public async Task UpdateBuildingAsync_ValidData_UpdatesBuildingSuccessfully()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding();
        var updateDto = CreateValidUpdateDto();

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidBuildingId))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateBuildingAsync(ValidBuildingId, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == ValidName)),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws BuildingNotFoundException.
    /// The repository should never be called with UpdateAsync.
    /// </summary>
    [Test]
    [Description("Application-002: UpdateBuildingAsync with non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistingBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var nonExistingId = 999;
        var updateDto = CreateValidUpdateDto();

        _mockRepository
            .Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Building?)null);

        // Act
        BuildingNotFoundException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(nonExistingId, updateDto);
        }
        catch (BuildingNotFoundException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected BuildingNotFoundException was not thrown");
            Assert.That(caughtException!.BuildingId, Is.EqualTo(nonExistingId));
        });
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates
    /// the domain validation exception (ArgumentException) and UpdateAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-003: UpdateBuildingAsync with invalid data propagates ArgumentException")]
    public async Task UpdateBuildingAsync_InvalidData_ThrowsArgumentException()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding();
        var invalidDto = new UpdateBuildingDto(
            "", ValidColor, ValidHeight, ValidLength,
            ValidWidth, ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidBuildingId))
            .ReturnsAsync(existingBuilding);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(ValidBuildingId, invalidDto);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("name"));
        });
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Building>()), Times.Never);
    }
}
