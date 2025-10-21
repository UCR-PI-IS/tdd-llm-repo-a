using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.ComponentsManagement;

public static class LearningComponentDtoMapper 
{
    public static LearningComponentDto ToDto(LearningComponent entity)
    {
        if (entity is Projector projector)
        {
            return ProjectorDtoMapper.ToDto(projector);
        }
        else if (entity is Whiteboard whiteboard)
        {
            return WhiteboardDtoMapper.ToDto(whiteboard);
        }
        else
        {
            throw new NotSupportedException($"Mapper for type {entity.GetType().Name} is not implemented.");
        }
    }
}





