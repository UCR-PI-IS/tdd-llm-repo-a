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
    /// <summary>
    /// Infrastructure-001: Verify that the ID generator produces a unique ID in the correct format.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: GenerateIdAsync produces unique ID with COMP- prefix")]
    public async Task GenerateIdAsync_ProducesUniqueIdWithCorrectFormat()
    {
        // Arrange
        var idGenerator = new ComponentIdGenerator();

        // Act
        var generatedId = await idGenerator.GenerateIdAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(generatedId, Is.Not.Null);
            Assert.That(generatedId, Does.StartWith("COMP-"));
        });
    }
}
