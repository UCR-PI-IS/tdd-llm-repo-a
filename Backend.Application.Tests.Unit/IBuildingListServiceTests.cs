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
    /// returning Task&lt;List&lt;Building&gt;&gt; with no parameters.
    /// </summary>
    [Test]
    [Description("Application-001: IBuildingListService defines GetAllBuildingsAsync returning Task<List<Building>>")]
    public void InterfaceContract_GetAllBuildingsAsync_IsCorrectlyDefined()
    {
        // Arrange & Act
        var method = typeof(IBuildingListService).GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null, "GetAllBuildingsAsync method should be defined on IBuildingListService");
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)));
            Assert.That(method.GetParameters(), Is.Empty);
        });
    }
}
