using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="PersonService.CreatePersonAsync"/>.
/// Covers intents Application-001 through Application-005.
/// </summary>
[TestFixture]
public class PersonServiceTests
{
    private Mock<IPersonRepository> _mockRepository = null!;
    private PersonService _sut = null!;

    // Valid test data
    private static readonly Person ValidPerson = new(
        1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _sut = new PersonService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify successful person creation when no duplicate email
    /// or identity number exists. The repository AddAsync is called exactly once.
    /// </summary>
    [Test]
    [Description("Application-001: Successful person creation when no duplicate exists")]
    public async Task CreatePersonAsync_NoDuplicateExists_ReturnsSuccess()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidPerson.Email))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidPerson.IdentityNumber))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreatePersonAsync(ValidPerson);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _mockRepository.Verify(r => r.AddAsync(ValidPerson), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that creation fails when a person with the same email
    /// already exists. The repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-002: Creation fails when duplicate email exists")]
    public async Task CreatePersonAsync_DuplicateEmailExists_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidPerson.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(ValidPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("A person with this email already exists."));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that creation fails when a person with the same identity number
    /// already exists. The repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-003: Creation fails when duplicate identity number exists")]
    public async Task CreatePersonAsync_DuplicateIdentityNumberExists_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidPerson.Email))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidPerson.IdentityNumber))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(ValidPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("A person with this identity number already exists."));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that creation fails when both email and identity number
    /// already exist. The repository AddAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-004: Creation fails when both email and identity number are duplicates")]
    public async Task CreatePersonAsync_BothDuplicatesExist_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidPerson.Email))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidPerson.IdentityNumber))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(ValidPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service handles repository exceptions gracefully,
    /// returning a failure result with the error message.
    /// </summary>
    [Test]
    [Description("Application-005: Service handles repository exceptions gracefully")]
    public async Task CreatePersonAsync_RepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidPerson.Email))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidPerson.IdentityNumber))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _sut.CreatePersonAsync(ValidPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Database connection failed"));
        });
    }
}
