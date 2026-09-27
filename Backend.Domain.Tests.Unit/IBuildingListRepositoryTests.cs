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
    /// Domain-002: Verify that the IBuildingListRepository interface defines
    /// a GetAllBuildingsAsync method returning Task&lt;List&lt;Building&gt;&gt; with no parameters.
    /// </summary>
    [Test]
    [Description("Domain-002: Verify IBuildingListRepository defines GetAllBuildingsAsync returning Task<List<Building>>")]
    public void GetAllBuildingsAsync_InterfaceContract_HasCorrectSignature()
    {
        // Arrange & Act
        var method = typeof(IBuildingListRepository).GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null, "GetAllBuildingsAsync method should exist on IBuildingListRepository");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)),
                "GetAllBuildingsAsync should return Task<List<Building>>");
            Assert.That(method.GetParameters(), Is.Empty,
                "GetAllBuildingsAsync should have no parameters");
        });
    }
}
