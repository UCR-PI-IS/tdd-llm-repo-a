using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints.MapCreateComponentEndpoint"/>.
/// Covers intent Presentation-005 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsTests
{
    private Mock<IEndpointRouteBuilder> _mockBuilder = null!;
    private Mock<IApplicationBuilder> _mockAppBuilder = null!;
    private Mock<ICreateLearningComponentHandler> _mockHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mockBuilder = new Mock<IEndpointRouteBuilder>();
        _mockAppBuilder = new Mock<IApplicationBuilder>();
        _mockHandler = new Mock<ICreateLearningComponentHandler>();

        // Setup the ApplicationBuilder property
        _mockBuilder.Setup(b => b.ServiceProvider).Returns(_mockAppBuilder.Object);
    }

    /// <summary>
    /// Presentation-005: Verify that the POST endpoint for creating components is correctly mapped.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify POST endpoint for creating components is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostEndpoint()
    {
        // Arrange
        var mockRouteHandlerBuilder = new Mock<IRouteHandlerBuilder>();

        _mockBuilder
            .Setup(b => b.MapPost("/api/components", It.IsAny<Delegate>()))
            .Returns(mockRouteHandlerBuilder.Object);

        mockRouteHandlerBuilder
            .Setup(r => r.WithName("CreateComponent"))
            .Returns(mockRouteHandlerBuilder.Object);

        mockRouteHandlerBuilder
            .Setup(r => r.WithOpenApi())
            .Returns(mockRouteHandlerBuilder.Object);

        // Act
        var result = LearningComponentEndpoints.MapCreateComponentEndpoint(
            _mockBuilder.Object, _mockHandler.Object);

        // Assert
        _mockBuilder.Verify(b => b.MapPost("/api/components", It.IsAny<Delegate>()), Times.Once);
    }

    /// <summary>
    /// Verify that the endpoint is named correctly.
    /// </summary>
    [Test]
    [Description("Verify endpoint is named 'CreateComponent'")]
    public void MapCreateComponentEndpoint_SetsCorrectName()
    {
        // Arrange
        var mockRouteHandlerBuilder = new Mock<IRouteHandlerBuilder>();

        _mockBuilder
            .Setup(b => b.MapPost("/api/components", It.IsAny<Delegate>()))
            .Returns(mockRouteHandlerBuilder.Object);

        mockRouteHandlerBuilder
            .Setup(r => r.WithName("CreateComponent"))
            .Returns(mockRouteHandlerBuilder.Object);

        mockRouteHandlerBuilder
            .Setup(r => r.WithOpenApi())
            .Returns(mockRouteHandlerBuilder.Object);

        // Act
        LearningComponentEndpoints.MapCreateComponentEndpoint(
            _mockBuilder.Object, _mockHandler.Object);

        // Assert
        mockRouteHandlerBuilder.Verify(r => r.WithName("CreateComponent"), Times.Once);
    }
}

/// <summary>
/// Interface for the create learning component handler.
/// </summary>
public interface ICreateLearningComponentHandler
{
    Task<IResult> HandleAsync(CreateLearningComponentDto dto);
}

/// <summary>
/// DTO for creating a learning component.
/// </summary>
public class CreateLearningComponentDto
{
    public string LearningSpaceId { get; set; } = string.Empty;
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string Orientation { get; set; } = string.Empty;
    public string? ComponentId { get; set; }
}
