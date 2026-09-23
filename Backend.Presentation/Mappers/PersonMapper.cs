using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps person-related DTOs to domain entities.
/// </summary>
internal static class PersonMapper
{
    /// <summary>
    /// Converts a <see cref="CreatePersonDto"/> to a <see cref="Person"/> domain entity.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="Person"/> instance.</returns>
    public static Person ToDomain(CreatePersonDto dto)
    {
        return new Person(0, dto.FirstName, dto.LastName, dto.Email, dto.IdentityNumber, dto.BirthDate);
    }
}
