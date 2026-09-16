using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ComponentIdGenerator.GenerateIdAsync"/>.
/// Covers CPD-LC-001-009 Infrastructure-001.
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
    /// CPD-LC-001-009 Infrastructure-001: Verify that the ID generator produces a non-null
    /// unique ID in the correct format starting with "COMP-".
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Infrastructure-001: ID generator produces non-null ID with COMP- prefix")]
    public async Task GenerateIdAsync_ReturnsNonNullIdWithCompPrefix()
    {
        // Arrange — no additional setup needed

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
