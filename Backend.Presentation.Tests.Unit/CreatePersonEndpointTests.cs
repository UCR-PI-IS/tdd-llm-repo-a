using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint"/> endpoint mapping.
/// Covers intent Presentation-011 for SPT-UM-001-003.
/// Note: This test uses a real WebApplication to verify endpoint mapping since
/// Moq cannot mock extension methods (MapPost on IEndpointRouteBuilder).
/// This file may need to be excluded from compilation if it causes build issues,
/// similar to LearningComponentEndpointsCreateTests.cs.
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
    public void MapEndpoint_PostPersons_EndpointIsMapped()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act
        CreatePersonEndpoint.MapEndpoint(app);

        // Assert
        var dataSource = app.DataSources.FirstOrDefault();
        var endpoints = dataSource?.Endpoints ?? new List<Endpoint>();
        var personEndpoint = endpoints.FirstOrDefault(e =>
            e.DisplayName?.Contains("CreatePerson") == true ||
            (e as RouteEndpoint)?.RoutePattern.RawText == "/api/persons");
        Assert.That(personEndpoint, Is.Not.Null);
    }
}
