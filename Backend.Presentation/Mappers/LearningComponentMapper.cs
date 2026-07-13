using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers
{
    /// <summary>
    /// Mapper for converting LearningComponent entities to DTOs.
    /// </summary>
    public static class LearningComponentMapper
    {
        /// <summary>
        /// Maps a LearningComponent entity to a LearningComponentDto.
        /// </summary>
        /// <param name="component">The learning component entity.</param>
        /// <returns>The mapped DTO.</returns>
        public static LearningComponentDto ToDto(LearningComponent component)
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
                component.Orientation
            );
        }

        /// <summary>
        /// Maps a collection of LearningComponent entities to a list of LearningComponentDto objects.
        /// </summary>
        /// <param name="components">The collection of learning component entities.</param>
        /// <returns>The list of mapped DTOs.</returns>
        public static List<LearningComponentDto> ToDtoList(IEnumerable<LearningComponent> components)
        {
            return components.Select(ToDto).ToList();
        }
    }
}
