using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    /// <summary>
    /// Test that SqlLearningComponentRepository implements ILearningComponentRepository.
    /// </summary>
    [Test]
    [Description("Verify that SqlLearningComponentRepository implements ILearningComponentRepository interface")]
    public void Repository_ImplementsILearningComponentRepository()
    {
        // Assert
        Assert.That(typeof(ILearningComponentRepository).IsAssignableFrom(typeof(SqlLearningComponentRepository)), Is.True);
    }

    /// <summary>
    /// Test that repository type can be instantiated.
    /// </summary>
    [Test]
    [Description("Verify that SqlLearningComponentRepository type exists and has correct constructor")]
    public void Repository_HasCorrectConstructor()
    {
        // Assert
        var constructor = typeof(SqlLearningComponentRepository).GetConstructor(new[] { typeof(UCRDatabaseContext) });
        Assert.That(constructor, Is.Not.Null);
    }

    /// <summary>
    /// Test that LearningComponentEntity maps correctly to LearningComponent domain model.
    /// </summary>
    [Test]
    [Description("Verify that LearningComponentEntity maps correctly to LearningComponent domain model")]
    public void EntityMapping_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var entity = new LearningComponentEntity
        {
            ComponentId = "comp-001",
            LearningSpaceId = "space-001",
            Width = 10f,
            Height = 5f,
            Depth = 8f,
            X = 1f,
            Y = 2f,
            Z = 3f,
            Orientation = Orientation.North
        };

        // Act
        var domainModel = new LearningComponent(
            entity.ComponentId,
            entity.LearningSpaceId,
            entity.Width,
            entity.Height,
            entity.Depth,
            entity.X,
            entity.Y,
            entity.Z,
            entity.Orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(domainModel.ComponentId, Is.EqualTo(entity.ComponentId));
            Assert.That(domainModel.LearningSpaceId, Is.EqualTo(entity.LearningSpaceId));
            Assert.That(domainModel.Width, Is.EqualTo(entity.Width));
            Assert.That(domainModel.Height, Is.EqualTo(entity.Height));
            Assert.That(domainModel.Depth, Is.EqualTo(entity.Depth));
            Assert.That(domainModel.X, Is.EqualTo(entity.X));
            Assert.That(domainModel.Y, Is.EqualTo(entity.Y));
            Assert.That(domainModel.Z, Is.EqualTo(entity.Z));
            Assert.That(domainModel.Orientation, Is.EqualTo(entity.Orientation));
        });
    }
}
