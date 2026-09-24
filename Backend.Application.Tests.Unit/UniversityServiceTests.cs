using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UniversityService.AddUniversityAsync"/>.
/// Covers intents Application-001 through Application-004.
/// </summary>
[TestFixture]
public class UniversityServiceTests
{
    private Mock<IUniversityRepository> _mockRepository = null!;
    private UniversityService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IUniversityRepository>();
        _sut = new UniversityService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that a valid university is successfully added through the service
    /// and returns a success result.
    /// </summary>
    [Test]
    [Description("Application-001: Service adds valid university and returns success")]
    public async Task AddUniversityAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync("Universidad de Costa Rica"))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<University>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddUniversityAsync("Universidad de Costa Rica", "Costa Rica");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _mockRepository.Verify(r => r.AddAsync(It.Is<University>(u =>
            u.Name == "Universidad de Costa Rica" && u.Country == "Costa Rica")), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a university with an existing name returns failure
    /// and does not call the repository AddAsync method.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate university name returns failure without calling AddAsync")]
    public async Task AddUniversityAsync_DuplicateName_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync("UCR"))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.AddUniversityAsync("UCR", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a university with null or empty name returns
    /// validation failure and does not call the repository AddAsync method.
    /// </summary>
    [Test]
    [Description("Application-003: Empty name returns validation failure without calling AddAsync")]
    public async Task AddUniversityAsync_EmptyName_ReturnsValidationFailure()
    {
        // Arrange
        // No repository setup needed — domain validation should fail before repository is called

        // Act
        var result = await _sut.AddUniversityAsync("", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that adding a university with null or empty country returns
    /// validation failure and does not call the repository AddAsync method.
    /// </summary>
    [Test]
    [Description("Application-004: Empty country returns validation failure without calling AddAsync")]
    public async Task AddUniversityAsync_EmptyCountry_ReturnsValidationFailure()
    {
        // Arrange
        // No repository setup needed — domain validation should fail before repository is called

        // Act
        var result = await _sut.AddUniversityAsync("Universidad de Costa Rica", "");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Country"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }
}
