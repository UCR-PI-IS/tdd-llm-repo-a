using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the Person entity.
/// Tests cover person creation validation and business rules.
/// </summary>
[TestFixture]
public class PersonTests
{
    [Test(Description = "Person should be created successfully with all valid fields including phone")]
    public void Constructor_WithAllValidFields_ShouldCreatePerson()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 1);
        var phone = "555-1234";

        // Act
        var person = new Person(id, firstName, lastName, email, identityNumber, birthDate, phone);

        // Assert
        Assert.That(person.Id, Is.EqualTo(id));
        Assert.That(person.FirstName, Is.EqualTo(firstName));
        Assert.That(person.LastName, Is.EqualTo(lastName));
        Assert.That(person.Email, Is.EqualTo(email));
        Assert.That(person.IdentityNumber, Is.EqualTo(identityNumber));
        Assert.That(person.BirthDate, Is.EqualTo(birthDate));
        Assert.That(person.Phone, Is.EqualTo(phone));
    }

    [Test(Description = "Person should be created successfully with null phone (optional field)")]
    public void Constructor_WithNullPhone_ShouldCreatePerson()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "Jane";
        var lastName = "Smith";
        var email = "jane.smith@example.com";
        var identityNumber = "987654321";
        var birthDate = new DateTime(1985, 5, 15);

        // Act
        var person = new Person(id, firstName, lastName, email, identityNumber, birthDate, null);

        // Assert
        Assert.That(person.Phone, Is.Null);
        Assert.That(person.Email, Is.EqualTo(email));
        Assert.That(person.FirstName, Is.EqualTo(firstName));
        Assert.That(person.LastName, Is.EqualTo(lastName));
    }

    [Test(Description = "Person constructor should throw ArgumentException when Email is null")]
    public void Constructor_WithNullEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 1);
        var phone = "555-1234";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var person = new Person(id, firstName, lastName, null, identityNumber, birthDate, phone);
        });
    }

    [Test(Description = "Person constructor should throw ArgumentException when FirstName is null")]
    public void Constructor_WithNullFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 1);
        var phone = "555-1234";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var person = new Person(id, null, lastName, email, identityNumber, birthDate, phone);
        });
    }

    [Test(Description = "Person constructor should throw ArgumentException when LastName is null")]
    public void Constructor_WithNullLastName_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 1);
        var phone = "555-1234";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var person = new Person(id, firstName, null, email, identityNumber, birthDate, phone);
        });
    }

    [Test(Description = "Person constructor should throw ArgumentException when IdentityNumber is null")]
    public void Constructor_WithNullIdentityNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var birthDate = new DateTime(1990, 1, 1);
        var phone = "555-1234";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var person = new Person(id, firstName, lastName, email, null, birthDate, phone);
        });
    }

    [Test(Description = "Person constructor should throw ArgumentException when BirthDate is in the future")]
    public void Constructor_WithFutureBirthDate_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var futureBirthDate = DateTime.Now.AddDays(1);
        var phone = "555-1234";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var person = new Person(id, firstName, lastName, email, identityNumber, futureBirthDate, phone);
        });
    }
}
