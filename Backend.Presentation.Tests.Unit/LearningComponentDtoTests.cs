using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the LearningComponentDto.
/// </summary>
[TestFixture]
public class LearningComponentDtoTests
{
    /// <summary>
    /// Verifies that LearningComponentDto can be created with valid parameters.
    /// </summary>
    [Test]
    [Description("Verify that LearningComponentDto can be created with valid parameters")]
    public void Constructor_WithValidParameters_CreatesDto()
    {
        // Arrange
        String componentId = "COMP-001";
        String learningSpaceId = "LS-001";

        // Act
        var dto = new LearningComponentDto(componentId, learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.ComponentId, Is.EqualTo(componentId));
            Assert.That(dto.LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }
}
