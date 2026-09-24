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
    [Description("Application-001: Service adds valid university and returns success")]
    public async Task AddUniversityAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var name = "Universidad de Costa Rica";
        var country = "Costa Rica";

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(name))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.Is<University>(u => u.Name == name && u.Country == country)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddUniversityAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            _mockRepository.Verify(r => r.AddAsync(It.Is<University>(u => u.Name == name && u.Country == country)), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that adding a university with an existing name returns failure.
    /// </summary>
    [Test]
    [Description("Application-002: Service returns failure when university name already exists")]
    public async Task AddUniversityAsync_DuplicateName_ReturnsFailure()
    {
        // Arrange
        var name = "UCR";
        var country = "Costa Rica";

        _mockRepository
            .Setup(r => r.ExistsByNameAsync(name))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.AddUniversityAsync(name, country);

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
    [TestCase(null, Description = "Application-003: Null name returns validation failure")]
    [TestCase("", Description = "Application-003: Empty name returns validation failure")]
    public async Task AddUniversityAsync_NullOrEmptyName_ReturnsValidationFailure(string? invalidName)
    {
        // Arrange — no repository setup needed; validation should fail before repository check

        // Act
        var result = await _sut.AddUniversityAsync(invalidName!, "Costa Rica");

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
    [TestCase(null, Description = "Application-004: Null country returns validation failure")]
    [TestCase("", Description = "Application-004: Empty country returns validation failure")]
    public async Task AddUniversityAsync_NullOrEmptyCountry_ReturnsValidationFailure(string? invalidCountry)
    {
        // Arrange — no repository setup needed; validation should fail before repository check

        // Act
        var result = await _sut.AddUniversityAsync("Universidad de Costa Rica", invalidCountry!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Country"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }
}
