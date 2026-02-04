using NUnit.Framework;
using System;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities
{
    [TestFixture]
    public class BuildingTests
    {
        [Test]
        [TestCase(1, "Engineering Building", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0, 
            Description = "Valid building creation with positive dimensions and coordinates")]
        [TestCase(2, "Science Building", "Green", 20.0, 40.0, 30.0, 150.0, 250.0, 5.0, 
            Description = "Valid building creation with different positive values")]
        [TestCase(3, "Arts Building", "Red", 10.0, 25.0, 15.0, 50.0, 75.0, 0.0, 
            Description = "Valid building creation with minimum positive values")]
        public void Constructor_WithValidParameters_CreatesBuilding(
            int internalId, string name, string color, double height, 
            double length, double width, double x, double y, double z)
        {
            // Act
            var building = new Building(internalId, name, color, height, length, width, x, y, z);

            // Assert
            Assert.That(building.InternalID, Is.EqualTo(internalId));
            Assert.That(building.Name, Is.EqualTo(name));
            Assert.That(building.Color, Is.EqualTo(color));
            Assert.That(building.Height, Is.EqualTo(height));
            Assert.That(building.Length, Is.EqualTo(length));
            Assert.That(building.Width, Is.EqualTo(width));
            Assert.That(building.X, Is.EqualTo(x));
            Assert.That(building.Y, Is.EqualTo(y));
            Assert.That(building.Z, Is.EqualTo(z));
        }

        [Test(Description = "Null name throws ArgumentNullException")]
        public void Constructor_WithNullName_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new Building(1, null, "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test(Description = "Empty name throws ArgumentException")]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                new Building(1, "", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.Message, Does.Contain("Name cannot be empty"));
        }

        [Test(Description = "Whitespace name throws ArgumentException")]
        public void Constructor_WithWhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                new Building(1, "   ", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.Message, Does.Contain("Name cannot be empty"));
        }

        [Test(Description = "Null color throws ArgumentNullException")]
        public void Constructor_WithNullColor_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new Building(1, "Engineering Building", null, 15.5, 30.0, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("color"));
        }

        [Test]
        [TestCase(-1.0, Description = "Negative height throws ArgumentOutOfRangeException")]
        [TestCase(-15.5, Description = "Large negative height throws ArgumentOutOfRangeException")]
        public void Constructor_WithNegativeHeight_ThrowsArgumentOutOfRangeException(double height)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", height, 30.0, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("height"));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test(Description = "Zero height throws ArgumentOutOfRangeException")]
        public void Constructor_WithZeroHeight_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", 0.0, 30.0, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("height"));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        [TestCase(-1.0, Description = "Negative length throws ArgumentOutOfRangeException")]
        [TestCase(-30.0, Description = "Large negative length throws ArgumentOutOfRangeException")]
        [TestCase(0.0, Description = "Zero length throws ArgumentOutOfRangeException")]
        public void Constructor_WithNonPositiveLength_ThrowsArgumentOutOfRangeException(double length)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", 15.5, length, 20.0, 100.0, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("length"));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        [TestCase(-1.0, Description = "Negative width throws ArgumentOutOfRangeException")]
        [TestCase(-20.0, Description = "Large negative width throws ArgumentOutOfRangeException")]
        [TestCase(0.0, Description = "Zero width throws ArgumentOutOfRangeException")]
        public void Constructor_WithNonPositiveWidth_ThrowsArgumentOutOfRangeException(double width)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", 15.5, 30.0, width, 100.0, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("width"));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        [TestCase(-1.0, Description = "Negative X coordinate throws ArgumentOutOfRangeException")]
        [TestCase(-100.0, Description = "Large negative X coordinate throws ArgumentOutOfRangeException")]
        public void Constructor_WithNegativeX_ThrowsArgumentOutOfRangeException(double x)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", 15.5, 30.0, 20.0, x, 200.0, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("x"));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }

        [Test]
        [TestCase(-1.0, Description = "Negative Y coordinate throws ArgumentOutOfRangeException")]
        [TestCase(-200.0, Description = "Large negative Y coordinate throws ArgumentOutOfRangeException")]
        public void Constructor_WithNegativeY_ThrowsArgumentOutOfRangeException(double y)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", 15.5, 30.0, 20.0, 100.0, y, 0.0));
            
            Assert.That(ex.ParamName, Is.EqualTo("y"));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }

        [Test]
        [TestCase(-1.0, Description = "Negative Z coordinate throws ArgumentOutOfRangeException")]
        [TestCase(-5.0, Description = "Large negative Z coordinate throws ArgumentOutOfRangeException")]
        public void Constructor_WithNegativeZ_ThrowsArgumentOutOfRangeException(double z)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Building(1, "Engineering Building", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, z));
            
            Assert.That(ex.ParamName, Is.EqualTo("z"));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }
    }
}
