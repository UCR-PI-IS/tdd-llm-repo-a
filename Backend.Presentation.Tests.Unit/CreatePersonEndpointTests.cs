using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
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
    private Mock<IEndpointRouteBuilder> _mockBuilder = null!;
    private Mock<IRouteHandlerBuilder> _mockRouteHandlerBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _mockBuilder = new Mock<IEndpointRouteBuilder>();
        _mockRouteHandlerBuilder = new Mock<IRouteHandlerBuilder>();

        _mockBuilder
            .Setup(x => x.MapPost(It.IsAny<string>(), It.IsAny<Delegate>()))
            .Returns(_mockRouteHandlerBuilder.Object);

        _mockRouteHandlerBuilder
            .Setup(x => x.WithName(It.IsAny<string>()))
            .Returns(_mockRouteHandlerBuilder.Object);
        _mockRouteHandlerBuilder
            .Setup(x => x.WithOpenApi())
            .Returns(_mockRouteHandlerBuilder.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockBuilder.VerifyAll();
        _mockRouteHandlerBuilder.VerifyAll();
    }

    /// <summary>
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// </summary>
    [Test]
    [Description("Presentation-011: Verify endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_MapsPostEndpoint()
    {
        // Act
        var result = CreatePersonEndpoint.MapEndpoint(_mockBuilder.Object);

        // Assert
        Assert.Multiple(() =>
        {
            _mockBuilder.Verify(x => x.MapPost("/api/persons", It.IsAny<Delegate>()), Times.Once);
            _mockRouteHandlerBuilder.Verify(x => x.WithName("CreatePerson"), Times.Once);
        });
    }
}
