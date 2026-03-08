using NUnit.Framework;
using System;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities
{
    [TestFixture]
    public class UniversityTests
    {
        [Test]
        [TestCase("UCR", "Costa Rica", Description = "Valid university creation with name and country")]
        [TestCase("MIT", "United States", Description = "Valid university creation with different values")]
        public void Constructor_ValidParameters_CreatesUniversity(string name, string country)
        {
            // Act
            var university = new University(0, name, country);

            // Assert
            Assert.That(university, Is.Not.Null);
            Assert.That(university.Name, Is.EqualTo(name));
            Assert.That(university.Country, Is.EqualTo(country));
        }

        [Test]
        [Description("Constructor throws ArgumentNullException when name is null")]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new University(0, null, "Costa Rica"));
            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        [Description("Constructor throws ArgumentException when name is empty")]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new University(0, "", "Costa Rica"));
            Assert.That(ex.Message, Does.Contain("name"));
        }

        [Test]
        [Description("Constructor throws ArgumentException when name is whitespace")]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new University(0, "   ", "Costa Rica"));
            Assert.That(ex.Message, Does.Contain("name"));
        }

        [Test]
        [Description("Constructor throws ArgumentNullException when country is null")]
        public void Constructor_NullCountry_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new University(0, "UCR", null));
            Assert.That(ex.ParamName, Is.EqualTo("country"));
        }

        [Test]
        [Description("Constructor throws ArgumentException when country is empty")]
        public void Constructor_EmptyCountry_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new University(0, "UCR", ""));
            Assert.That(ex.Message, Does.Contain("country"));
        }

        [Test]
        [Description("Constructor throws ArgumentException when country is whitespace")]
        public void Constructor_WhitespaceCountry_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new University(0, "UCR", "   "));
            Assert.That(ex.Message, Does.Contain("country"));
        }
    }
}
