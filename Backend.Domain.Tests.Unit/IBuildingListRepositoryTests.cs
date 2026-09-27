using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="IBuildingListRepository"/> interface contract.
/// Covers intent Domain-002.
/// </summary>
[TestFixture]
public class IBuildingListRepositoryTests
{
    /// <summary>
    /// Domain-002: Verify repository interface defines GetAllBuildingsAsync method
    /// returning Task&lt;List&lt;Building&gt;&gt; with no parameters.
    /// </summary>
    [Test]
    [Description("Domain-002: IBuildingListRepository defines GetAllBuildingsAsync with correct signature")]
    public void GetAllBuildingsAsync_InterfaceContract_DefinesCorrectSignature()
    {
        // Arrange
        var interfaceType = typeof(IBuildingListRepository);

        // Act
        var method = interfaceType.GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null, "GetAllBuildingsAsync method should be defined");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)));
            Assert.That(method.GetParameters(), Is.Empty);
        });
    }
}
