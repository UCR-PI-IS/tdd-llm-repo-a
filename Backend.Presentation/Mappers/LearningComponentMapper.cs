using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers
{
    /// <summary>
    /// Maps LearningComponent entities to DTOs and responses.
    /// </summary>
    public static class LearningComponentMapper
    {
        /// <summary>
        /// Maps a list of LearningComponent entities to a GetLearningComponentsResponse.
        /// </summary>
        /// <param name="components">The list of learning components.</param>
        /// <returns>A response containing the list of component DTOs.</returns>
        public static GetLearningComponentsResponse MapToResponse(List<LearningComponent> components)
        {
            var dtos = new List<LearningComponentDto>(components.Count);
            foreach (var component in components)
            {
                dtos.Add(MapToDto(component));
            }
            return new GetLearningComponentsResponse(dtos);
        }

        /// <summary>
        /// Maps a single LearningComponent entity to a LearningComponentDto.
        /// </summary>
        /// <param name="component">The learning component entity.</param>
        /// <returns>A DTO representing the component.</returns>
        public static LearningComponentDto MapToDto(LearningComponent component)
        {
            return new LearningComponentDto(
                component.ComponentId,
                component.LearningSpaceId,
                component.Width,
                component.Height,
                component.Depth,
                component.X,
                component.Y,
                component.Z,
                component.Orientation);
        }
    }
}
