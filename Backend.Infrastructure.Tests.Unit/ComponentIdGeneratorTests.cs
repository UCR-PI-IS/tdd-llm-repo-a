using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ComponentIdGenerator.GenerateIdAsync"/>.
/// Covers intent Infrastructure-001.
/// </summary>
[TestFixture]
public class ComponentIdGeneratorTests
{
    private ComponentIdGenerator _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new ComponentIdGenerator();
    }

    /// <summary>
    /// Infrastructure-001: Verify that the ID generator produces a unique ID in the correct format.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify ID generator produces unique ID in correct format")]
    public async Task GenerateIdAsync_ReturnsUniqueIdInCorrectFormat()
    {
        // Act
        var id1 = await _sut.GenerateIdAsync();
        var id2 = await _sut.GenerateIdAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(id1, Is.Not.Null.And.Not.Empty);
            Assert.That(id1, Does.StartWith("COMP-"));
            Assert.That(id2, Is.Not.Null.And.Not.Empty);
            Assert.That(id2, Does.StartWith("COMP-"));
            Assert.That(id1, Is.Not.EqualTo(id2), "Generated IDs should be unique");
        });
    }
}
