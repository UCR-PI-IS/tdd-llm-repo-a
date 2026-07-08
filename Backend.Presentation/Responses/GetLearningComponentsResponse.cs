using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Response for getting learning components.
/// </summary>
public class GetLearningComponentsResponse
{
    /// <summary>
    /// List of learning components.
    /// </summary>
    public List<LearningComponentDto> Components { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLearningComponentsResponse"/> class.
    /// </summary>
    /// <param name="components">The list of learning components</param>
    public GetLearningComponentsResponse(List<LearningComponentDto> components)
    {
        Components = components;
    }

    /// <summary>
    /// Creates a <see cref="GetLearningComponentsResponse"/> from domain entities.
    /// </summary>
    public static GetLearningComponentsResponse FromDomain(List<LearningComponent> components) =>
        new(components.Select(LearningComponentDto.FromDomain).ToList());

    /// <summary>
    /// Creates an <see cref="IResult"/> with OK status containing the response.
    /// </summary>
    public static IResult OkResult(List<LearningComponent> components) =>
        Results.Ok(FromDomain(components));
}
