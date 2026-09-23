using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="PersonService.CreatePersonAsync"/>.
/// Covers intents Application-001 through Application-005 for SPT-UM-001-003.
/// </summary>
[TestFixture]
public class PersonServiceTests
{
    private Mock<IPersonRepository> _mockRepository = null!;
    private PersonService _sut = null!;

    // Valid test data
    private const int ValidId = 1;
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new(1990, 1, 15);

    private Person _validPerson = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _sut = new PersonService(_mockRepository.Object);
        _validPerson = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify successful person creation when no duplicate exists.
    /// The service should add the person to the repository and return a success result.
    /// </summary>
    [Test]
    [Description("Application-001: Successful person creation when no duplicate exists")]
    public async Task CreatePersonAsync_NoDuplicateExists_ReturnsSuccess()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidEmail))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(_validPerson))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreatePersonAsync(_validPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            _mockRepository.Verify(r => r.AddAsync(_validPerson), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that creation fails when a person with the same email already exists.
    /// The service should not add the person and return a failure result with the appropriate error message.
    /// </summary>
    [Test]
    [Description("Application-002: Creation fails when duplicate email exists")]
    public async Task CreatePersonAsync_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidEmail))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(_validPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("A person with this email already exists."));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that creation fails when a person with the same identity number already exists.
    /// The service should not add the person and return a failure result with the appropriate error message.
    /// </summary>
    [Test]
    [Description("Application-003: Creation fails when duplicate identity number exists")]
    public async Task CreatePersonAsync_DuplicateIdentityNumber_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidEmail))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(_validPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("A person with this identity number already exists."));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-004: Verify that creation fails when both email and identity number already exist.
    /// The service should not add the person and return a failure result indicating duplication.
    /// </summary>
    [Test]
    [Description("Application-004: Creation fails when both email and identity number are duplicates")]
    public async Task CreatePersonAsync_BothDuplicatesExist_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidEmail))
            .ReturnsAsync(true);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(_validPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-005: Verify that the service handles repository exceptions gracefully.
    /// When the repository throws an exception, the service should return a failure result
    /// with the exception message.
    /// </summary>
    [Test]
    [Description("Application-005: Service handles repository exceptions gracefully")]
    public async Task CreatePersonAsync_RepositoryException_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByEmailAsync(ValidEmail))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(_validPerson))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _sut.CreatePersonAsync(_validPerson);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Database connection failed"));
        });
    }
}
