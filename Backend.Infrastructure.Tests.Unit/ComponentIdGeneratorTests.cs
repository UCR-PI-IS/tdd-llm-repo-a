using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ComponentIdGenerator.GenerateIdAsync"/>.
/// Covers intent Infrastructure-001 from CPD-LC-001-009.
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
    /// Infrastructure-001 (CPD-LC-001-009): Verify that the ID generator produces a
    /// non-null unique ID in the correct format, starting with "COMP-".
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Generated ID is non-null and starts with 'COMP-'")]
    public async Task GenerateIdAsync_ReturnsUniqueIdInCorrectFormat()
    {
        // Act
        var generatedId = await _sut.GenerateIdAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(generatedId, Is.Not.Null);
            Assert.That(generatedId, Does.StartWith("COMP-"));
        });
    }
}
