using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit
{
    [TestFixture]
    public class LearningComponentTests
    {
        private Guid _componentId;
        private Guid _learningSpaceId;
        private float _width;
        private float _height;
        private float _depth;
        private float _x;
        private float _y;
        private float _z;
        private string _orientation;

        [SetUp]
        public void SetUp()
        {
            _componentId = Guid.NewGuid();
            _learningSpaceId = Guid.NewGuid();
            _width = 10.0f;
            _height = 5.0f;
            _depth = 2.0f;
            _x = 1.0f;
            _y = 2.0f;
            _z = 3.0f;
            _orientation = "North";
        }

        [Test]
        [Description("Verify that a LearningComponent entity can be created with valid parameters")]
        public void Constructor_ValidParameters_CreatesEntityWithExpectedProperties()
        {
            // Arrange
            // values from SetUp

            // Act
            var component = new LearningComponent(
                _componentId,
                _learningSpaceId,
                _width,
                _height,
                _depth,
                _x,
                _y,
                _z,
                _orientation);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(component.ComponentId, Is.EqualTo(_componentId));
                Assert.That(component.LearningSpaceId, Is.EqualTo(_learningSpaceId));
                Assert.That(component.Width, Is.EqualTo(_width));
                Assert.That(component.Height, Is.EqualTo(_height));
                Assert.That(component.Depth, Is.EqualTo(_depth));
                Assert.That(component.X, Is.EqualTo(_x));
                Assert.That(component.Y, Is.EqualTo(_y));
                Assert.That(component.Z, Is.EqualTo(_z));
                Assert.That(component.Orientation, Is.EqualTo(_orientation));
            });
        }

        [TestCase(-1.0f, "width", Description = "Verify that creating a LearningComponent with negative width throws ArgumentException")]
        [TestCase(-1.0f, "height", Description = "Verify that creating a LearningComponent with negative height throws ArgumentException")]
        [TestCase(-1.0f, "depth", Description = "Verify that creating a LearningComponent with negative depth throws ArgumentException")]
        [TestCase(-1.0f, "x", Description = "Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
        [TestCase(-1.0f, "y", Description = "Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
        [TestCase(-1.0f, "z", Description = "Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
        public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(float invalidValue, string paramName)
        {
            // Arrange
            var width = paramName == "width" ? invalidValue : _width;
            var height = paramName == "height" ? invalidValue : _height;
            var depth = paramName == "depth" ? invalidValue : _depth;
            var x = paramName == "x" ? invalidValue : _x;
            var y = paramName == "y" ? invalidValue : _y;
            var z = paramName == "z" ? invalidValue : _z;

            // Act & Assert
            Assert.That(
                () => new LearningComponent(
                    _componentId,
                    _learningSpaceId,
                    width,
                    height,
                    depth,
                    x,
                    y,
                    z,
                    _orientation),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo(paramName));
        }

        [Test]
        [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
        public void Constructor_InvalidOrientation_ThrowsArgumentException()
        {
            // Arrange
            const string invalidOrientation = "Northeast";

            // Act & Assert
            Assert.That(
                () => new LearningComponent(
                    _componentId,
                    _learningSpaceId,
                    _width,
                    _height,
                    _depth,
                    _x,
                    _y,
                    _z,
                    invalidOrientation),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("orientation"));
        }

        [TestCase("North", Description = "Verify that creating a LearningComponent with orientation North succeeds")]
        [TestCase("South", Description = "Verify that creating a LearningComponent with orientation South succeeds")]
        [TestCase("East", Description = "Verify that creating a LearningComponent with orientation East succeeds")]
        [TestCase("West", Description = "Verify that creating a LearningComponent with orientation West succeeds")]
        public void Constructor_ValidOrientation_SetsOrientation(string orientation)
        {
            // Arrange
            // orientation from TestCase

            // Act
            var component = new LearningComponent(
                _componentId,
                _learningSpaceId,
                _width,
                _height,
                _depth,
                _x,
                _y,
                _z,
                orientation);

            // Assert
            Assert.That(component.Orientation, Is.EqualTo(orientation));
        }

        [Test]
        [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds (boundary test)")]
        public void Constructor_ZeroDimensionsAndCoordinates_Succeeds()
        {
            // Arrange
            const float zero = 0.0f;

            // Act
            var component = new LearningComponent(
                _componentId,
                _learningSpaceId,
                zero,
                zero,
                zero,
                zero,
                zero,
                zero,
                _orientation);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(component.Width, Is.EqualTo(0f));
                Assert.That(component.Height, Is.EqualTo(0f));
                Assert.That(component.Depth, Is.EqualTo(0f));
                Assert.That(component.X, Is.EqualTo(0f));
                Assert.That(component.Y, Is.EqualTo(0f));
                Assert.That(component.Z, Is.EqualTo(0f));
            });
        }
    }
}
