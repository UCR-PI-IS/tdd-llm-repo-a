using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonMapper
{
    public static Person ToDomain(CreatePersonDto dto)
    {
        return new Person(0, dto.FirstName, dto.LastName, dto.Email, dto.IdentityNumber, dto.BirthDate);
    }
}
