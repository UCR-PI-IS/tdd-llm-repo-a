using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects.AccountManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota.Models;


namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Mappers;

internal static  class RoleDtoMapper
{
    public static Role ToEntity(this RoleDto dto)
    {
        return new Role(dto.Name)
        {
            Id = dto.Id!.Value
        };
    }

}
