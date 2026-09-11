using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ComponentIdGenerator.GenerateIdAsync"/>.
/// Covers intent Infrastructure-001 for CPD-LC-001-009.
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
    [Description("Infrastructure-001: Verify that ID generator produces unique ID in correct format")]
    public async Task GenerateIdAsync_ReturnsUniqueIdWithCorrectFormat()
    {
        // Act
        var generatedId = await _sut.GenerateIdAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(generatedId, Is.Not.Null);
            Assert.That(generatedId, Is.Not.Empty);
            Assert.That(generatedId, Does.StartWith("COMP-"));
        });
    }

    /// <summary>
    /// Verify that multiple calls to GenerateIdAsync produce different IDs.
    /// </summary>
    [Test]
    [Description("Verify that multiple calls produce different unique IDs")]
    public async Task GenerateIdAsync_MultipleCalls_ReturnDifferentIds()
    {
        // Act
        var id1 = await _sut.GenerateIdAsync();
        var id2 = await _sut.GenerateIdAsync();
        var id3 = await _sut.GenerateIdAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(id1, Is.Not.EqualTo(id2));
            Assert.That(id2, Is.Not.EqualTo(id3));
            Assert.That(id1, Is.Not.EqualTo(id3));
        });
    }
}
