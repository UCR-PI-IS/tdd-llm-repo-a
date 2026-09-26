using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="IBuildingListRepository"/> interface contract.
/// Covers intent Domain-002.
/// </summary>
[TestFixture]
public class IBuildingListRepositoryTests
{
    /// <summary>
    /// Domain-002: Verify repository interface defines GetAllBuildingsAsync method returning Task&lt;List&lt;Building&gt;&gt;.
    /// </summary>
    [Test]
    [Description("Domain-002: IBuildingListRepository defines GetAllBuildingsAsync with correct signature")]
    public void InterfaceContract_GetAllBuildingsAsync_IsDefinedCorrectly()
    {
        // Arrange & Act
        var method = typeof(IBuildingListRepository).GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null);
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)));
            Assert.That(method.GetParameters(), Is.Empty);
        });
    }
}
