using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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

    private const string ValidName = "Universidad de Costa Rica";
    private const string ValidCountry = "Costa Rica";

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
    /// Application-001: Verify that a valid university is successfully added through the service.
    /// </summary>
    [Test]
    [Description("Application-001: Service adds university successfully when no duplicate exists")]
    public async Task AddUniversityAsync_ValidUniversity_ReturnsSuccess()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<University>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            _mockRepository.Verify(r => r.AddAsync(It.Is<University>(u =>
                u.Name == ValidName && u.Country == ValidCountry)), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that adding a university with an existing name returns failure.
    /// </summary>
    [Test]
    [Description("Application-002: Service returns failure when university with same name already exists")]
    public async Task AddUniversityAsync_DuplicateName_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that adding a university with null or empty name returns validation failure.
    /// </summary>
    [Test]
    [Description("Application-003: Service returns validation failure when name is empty")]
    public async Task AddUniversityAsync_EmptyName_ReturnsValidationFailure()
    {
        // Arrange
        // No repository setup needed — validation should fail before repository is called

        // Act
        var result = await _sut.AddUniversityAsync("", ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-004: Verify that adding a university with null or empty country returns validation failure.
    /// </summary>
    [Test]
    [Description("Application-004: Service returns validation failure when country is empty")]
    public async Task AddUniversityAsync_EmptyCountry_ReturnsValidationFailure()
    {
        // Arrange
        // No repository setup needed — validation should fail before repository is called

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, "");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Country"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }
}
