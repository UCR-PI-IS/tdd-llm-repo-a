using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="IBuildingListService"/> interface contract.
/// Covers intent Application-001.
/// </summary>
[TestFixture]
public class IBuildingListServiceTests
{
    /// <summary>
    /// Application-001: Verify service interface defines GetAllBuildingsAsync method
    /// that returns Task&lt;List&lt;Building&gt;&gt; with no parameters.
    /// </summary>
    [Test]
    [Description("Application-001: IBuildingListService defines GetAllBuildingsAsync with correct signature")]
    public void GetAllBuildingsAsync_InterfaceContract_DefinesCorrectSignature()
    {
        // Arrange
        var interfaceType = typeof(IBuildingListService);

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
