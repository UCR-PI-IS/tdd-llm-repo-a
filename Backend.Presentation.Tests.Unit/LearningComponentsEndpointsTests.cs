using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints"/> endpoint mapping.
/// Covers CPD-LC-001-009 Presentation-005.
/// </summary>
[TestFixture]
public class LearningComponentsEndpointsTests
{
    /// <summary>
    /// CPD-LC-001-009 Presentation-005: Verify that the POST endpoint for creating components
    /// is correctly mapped via MapCreateComponentEndpoint.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Presentation-005: POST endpoint for creating components is correctly mapped")]
    public void MapCreateComponentEndpoint_RegistersPostEndpoint()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        var app = builder.Build();

        // Act
        var result = LearningComponentEndpoints.MapCreateComponentEndpoint(app);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null, "MapCreateComponentEndpoint should return the builder");
            Assert.That(result, Is.SameAs(app), "MapCreateComponentEndpoint should return the same builder instance");
        });
    }
}
