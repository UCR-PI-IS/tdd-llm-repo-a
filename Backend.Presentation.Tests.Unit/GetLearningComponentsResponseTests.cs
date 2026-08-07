using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the GetLearningComponentsResponse.
/// </summary>
[TestFixture]
public class GetLearningComponentsResponseTests
{
    /// <summary>
    /// Verifies that GetLearningComponentsResponse can be created with a list of components.
    /// </summary>
    [Test]
    [Description("Verify that GetLearningComponentsResponse can be created with a list of components")]
    public void Constructor_WithComponents_CreatesResponse()
    {
        // Arrange
        var components = new List<LearningComponentDto>
        {
            new LearningComponentDto("COMP-001", "LS-001"),
            new LearningComponentDto("COMP-002", "LS-001")
        };

        // Act
        var response = new GetLearningComponentsResponse(components);

        // Assert
        Assert.That(response.Components.Count, Is.EqualTo(2));
    }

    /// <summary>
    /// Verifies that GetLearningComponentsResponse can be created with an empty list.
    /// </summary>
    [Test]
    [Description("Verify that GetLearningComponentsResponse can be created with an empty list")]
    public void Constructor_WithEmptyList_CreatesResponse()
    {
        // Arrange
        var components = new List<LearningComponentDto>();

        // Act
        var response = new GetLearningComponentsResponse(components);

        // Assert
        Assert.That(response.Components, Is.Empty);
    }
}
