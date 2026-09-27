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
    /// Application-001: Verify that the IBuildingListService interface defines
    /// a GetAllBuildingsAsync method returning Task&lt;List&lt;Building&gt;&gt; with no parameters.
    /// </summary>
    [Test]
    [Description("Application-001: Verify IBuildingListService defines GetAllBuildingsAsync returning Task<List<Building>>")]
    public void GetAllBuildingsAsync_InterfaceContract_HasCorrectSignature()
    {
        // Arrange & Act
        var method = typeof(IBuildingListService).GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null, "GetAllBuildingsAsync method should exist on IBuildingListService");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)),
                "GetAllBuildingsAsync should return Task<List<Building>>");
            Assert.That(method.GetParameters(), Is.Empty,
                "GetAllBuildingsAsync should have no parameters");
        });
    }
}
