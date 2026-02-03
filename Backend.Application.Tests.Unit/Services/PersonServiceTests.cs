using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for the PersonService.
/// Tests cover person creation business logic including duplicate validation.
/// </summary>
[TestFixture]
public class PersonServiceTests
{
    private Mock<IPersonRepository> _mockRepository;
    private IPersonService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _service = new PersonService(_mockRepository.Object);
    }

    [Test(Description = "CreatePersonAsync should successfully create person when no duplicates exist")]
    public async Task CreatePersonAsync_WithValidDataAndNoDuplicates_ShouldReturnTrue()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 1);
        var phone = "555-1234";

        _mockRepository
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync((Person)null);

        _mockRepository
            .Setup(r => r.GetByIdentityNumberAsync(identityNumber))
            .ReturnsAsync((Person)null);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreatePersonAsync(firstName, lastName, email, identityNumber, birthDate, phone);

        // Assert
        Assert.That(result, Is.True);
        _mockRepository.Verify(r => r.GetByEmailAsync(email), Times.Once);
        _mockRepository.Verify(r => r.GetByIdentityNumberAsync(identityNumber), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Once);
    }

    [Test(Description = "CreatePersonAsync should throw InvalidOperationException when email already exists")]
    public async Task CreatePersonAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingPerson = new Person(
            Guid.NewGuid(),
            "Jane",
            "Smith",
            "duplicate@example.com",
            "111111111",
            new DateTime(1985, 1, 1),
            null
        );

        _mockRepository
            .Setup(r => r.GetByEmailAsync("duplicate@example.com"))
            .ReturnsAsync(existingPerson);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _service.CreatePersonAsync(
                "John",
                "Doe",
                "duplicate@example.com",
                "123456789",
                new DateTime(1990, 1, 1),
                "555-1234"
            );
        });

        _mockRepository.Verify(r => r.GetByEmailAsync("duplicate@example.com"), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    [Test(Description = "CreatePersonAsync should throw InvalidOperationException when identity number already exists")]
    public async Task CreatePersonAsync_WithDuplicateIdentityNumber_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingPerson = new Person(
            Guid.NewGuid(),
            "Jane",
            "Smith",
            "jane@example.com",
            "999999999",
            new DateTime(1985, 1, 1),
            null
        );

        _mockRepository
            .Setup(r => r.GetByEmailAsync("john.doe@example.com"))
            .ReturnsAsync((Person)null);

        _mockRepository
            .Setup(r => r.GetByIdentityNumberAsync("999999999"))
            .ReturnsAsync(existingPerson);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _service.CreatePersonAsync(
                "John",
                "Doe",
                "john.doe@example.com",
                "999999999",
                new DateTime(1990, 1, 1),
                "555-1234"
            );
        });

        _mockRepository.Verify(r => r.GetByEmailAsync("john.doe@example.com"), Times.Once);
        _mockRepository.Verify(r => r.GetByIdentityNumberAsync("999999999"), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    [Test(Description = "CreatePersonAsync should throw ArgumentException when required field is missing")]
    public async Task CreatePersonAsync_WithMissingRequiredField_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreatePersonAsync(
                null, // Missing FirstName
                "Doe",
                "john.doe@example.com",
                "123456789",
                new DateTime(1990, 1, 1),
                "555-1234"
            );
        });

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }
}
