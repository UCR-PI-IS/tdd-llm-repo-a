using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonEntityFactory
{
    public static Person Create(CreatePersonDto dto)
    {
        return new Person(
            Guid.NewGuid(),
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.IdentityNumber,
            dto.BirthDate);
    }
}
