using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.ComponentsManagement;

public static class LearningComponentNoIdDtoMapper
{
    public static LearningComponentNoIdDto ToDto(LearningComponent entity)
    {
        if (entity is Projector projector)
        {
            return ProjectorNoIdDtoMapper.ToDto(projector);
        }
        else if (entity is Whiteboard whiteboard)
        {
            return WhiteboardNoIdDtoMapper.ToDto(whiteboard);
        }
        else
        {
            throw new NotSupportedException($"Mapper for type {entity.GetType().Name} is not implemented.");
        }
    }
}
