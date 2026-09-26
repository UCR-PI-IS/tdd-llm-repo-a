using System.Reflection;
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
    /// Domain-002: Verify that the repository interface defines GetAllBuildingsAsync
    /// method returning Task&lt;List&lt;Building&gt;&gt; with no parameters.
    /// </summary>
    [Test]
    [Description("Domain-002: IBuildingListRepository defines GetAllBuildingsAsync returning Task<List<Building>>")]
    public void Interface_DefinesGetAllBuildingsAsync_WithCorrectSignature()
    {
        // Arrange
        var method = typeof(IBuildingListRepository).GetMethod(
            "GetAllBuildingsAsync",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null, "GetAllBuildingsAsync method must be defined");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)));
            Assert.That(method.GetParameters(), Is.Empty);
        });
    }
}
