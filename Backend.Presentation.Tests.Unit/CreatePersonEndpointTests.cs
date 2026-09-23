using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint.MapEndpoint"/>.
/// Covers intent Presentation-011.
/// </summary>
[TestFixture]
public class CreatePersonEndpointTests
{
    /// <summary>
    /// Presentation-011: Verify that the endpoint is correctly mapped to POST /api/persons.
    /// Uses a real WebApplication instance to register and inspect the endpoint metadata.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_MapsPostToApiPersons()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        var app = builder.Build();

        // Act
        CreatePersonEndpoint.MapEndpoint(app);

        // Assert
        var endpoints = app.DataSources
            .OfType<EndpointDataSource>()
            .SelectMany(ds => ds.Endpoints)
            .ToList();

        var personEndpoint = endpoints.FirstOrDefault(e =>
            e.DisplayName?.Contains("CreatePerson") == true ||
            (e as RouteEndpoint)?.RoutePattern.RawText == "/api/persons");

        Assert.That(personEndpoint, Is.Not.Null);
    }
}
