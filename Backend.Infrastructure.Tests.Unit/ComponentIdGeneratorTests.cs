using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ComponentIdGenerator.GenerateIdAsync"/>.
/// Covers intent Infrastructure-001 for story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class ComponentIdGeneratorTests
{
    private IComponentIdGenerator _idGenerator = null!;

    [SetUp]
    public void SetUp()
    {
        _idGenerator = new ComponentIdGenerator();
    }

    /// <summary>
    /// Infrastructure-001: Verify that the ID generator produces a unique ID in the correct format.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify that the ID generator produces a unique ID in the correct format")]
    public async Task GenerateIdAsync_ReturnsUniqueIdInCorrectFormat()
    {
        // Arrange & Act
        var generatedId = await _idGenerator.GenerateIdAsync();

        // Assert
        Assert.That(generatedId, Is.Not.Null);
        Assert.That(generatedId, Does.StartWith("COMP-"));
    }
}
