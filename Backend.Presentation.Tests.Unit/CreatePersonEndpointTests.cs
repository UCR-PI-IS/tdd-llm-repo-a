using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint"/> endpoint mapping.
/// Covers intent Presentation-011 for SPT-UM-001-003.
/// Note: This test may need to be excluded from compilation via csproj if
/// Moq cannot mock extension methods on IEndpointRouteBuilder (same limitation
/// as LearningComponentEndpointsCreateTests.cs).
/// </summary>
[TestFixture]
public class CreatePersonEndpointTests
{
    /// <summary>
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// Uses a real WebApplication to map endpoints and verify the route exists.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_PostPersons_IsMapped()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act
        app.MapCreatePersonEndpoint();
        var endpoints = (app as IEndpointRouteBuilder)
            .DataSources
            .SelectMany(ds => ds.Endpoints)
            .ToList();

        // Assert
        var personEndpoint = endpoints.FirstOrDefault(e =>
            e.DisplayName?.Contains("CreatePerson") == true ||
            (e as RouteEndpoint)?.RoutePattern.RawText == "/api/persons");
        Assert.That(personEndpoint, Is.Not.Null);
    }
}
