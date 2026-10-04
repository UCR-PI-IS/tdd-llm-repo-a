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
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// Uses a real WebApplication to verify the endpoint registration since
    /// Moq cannot mock extension methods like MapPost on IEndpointRouteBuilder.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_PostPersons_EndpointIsRegistered()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        var app = builder.Build();

        // Act
        CreatePersonEndpoint.MapEndpoint(app);

        // Assert
        var endpoints = app.DataSources
            .SelectMany(ds => ds.Endpoints)
            .ToList();

        var personEndpoint = endpoints.FirstOrDefault(e =>
            e.DisplayName?.Contains("CreatePerson") == true ||
            (e as RouteEndpoint)?.RoutePattern.RawText == "/api/persons");

        Assert.That(personEndpoint, Is.Not.Null);
    }
}
