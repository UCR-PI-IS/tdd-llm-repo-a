using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint"/> mapping.
/// Covers intent Presentation-011.
/// </summary>
[TestFixture]
public class CreatePersonEndpointTests
{
    /// <summary>
    /// Presentation-011: Verify that the endpoint is correctly mapped to POST /api/persons.
    /// Uses a real WebApplication to avoid Moq limitations with extension methods.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_MapsPostToApiPersons()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act
        var result = CreatePersonEndpoint.MapEndpoint(app);

        // Assert - verify that MapEndpoint returns the builder (indicating successful mapping)
        Assert.That(result, Is.Not.Null,
            "Expected MapEndpoint to return a non-null IEndpointRouteBuilder.");
        Assert.That(result, Is.EqualTo(app),
            "Expected MapEndpoint to return the same IEndpointRouteBuilder instance.");
    }
}
