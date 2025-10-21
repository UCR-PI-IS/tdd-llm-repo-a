using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers.AccountManagement;

/// <summary>
/// Provides mapping logic between domain entities and <see cref="UserWithPersonDto"/> representations.
/// </summary>
internal static class UserWithPersonDtoMapper
{
    /// <summary>
    /// Maps the specified <see cref="User"/>, <see cref="Person"/>, and list of <see cref="Role"/> entities
    /// to a <see cref="UserWithPersonDto"/> data transfer object.
    /// </summary>
    /// <param name="user">The user entity containing user-specific information.</param>
    /// <param name="person">The person entity containing personal information associated with the user.</param>
    /// <param name="roles">A list of roles assigned to the user.</param>
    /// <returns>
    /// A <see cref="UserWithPersonDto"/> instance populated with the combined data from the user, person, and roles.
    /// </returns>
    internal static UserWithPerson ToEntity(CreateUserWithPersonDto dto)
    {
        return new UserWithPerson(
            UserName.Create(dto.UserName),
            dto.FirstName,
            dto.LastName,
            Email.Create(dto.Email),
            Phone.Create(dto.Phone),
            IdentityNumber.Create(dto.IdentityNumber),
            BirthDate.Create(dto.BirthDate),  
            roles: dto.Roles ?? new List<string>()
        );
    }

    /// <summary>
    /// Updates the specified <see cref="UserWithPerson"/> entity with values from the provided <see cref="CreateUserWithPersonDto"/>.
    /// </summary>
    /// <param name="entity"> The <see cref="UserWithPerson"/> entity to be updated.</param>
    /// <param name="dto"> The <see cref="CreateUserWithPersonDto"/> containing the new values.</param>
    /// <return> void</return>
    internal static void UpdateEntity(UserWithPerson entity, CreateUserWithPersonDto dto)
    {
        if (dto.UserName != default)
        {
            entity.UserName = UserName.Create(dto.UserName);
        }
        if (dto.IdentityNumber != default)
        {
            entity.IdentityNumber = IdentityNumber.Create(dto.IdentityNumber);
        }
        if (dto.BirthDate != default)
        {
            entity.BirthDate = BirthDate.Create(dto.BirthDate);
        }
        if (dto.Email != default)
        {
            entity.Email = Email.Create(dto.Email);
        }
        if (dto.Phone != default)
        {
            entity.Phone = Phone.Create(dto.Phone);
        }

        if (dto.FirstName != default)
        {
            entity.FirstName = dto.FirstName;
        }
        if (dto.LastName != default)
        {
            entity.LastName = dto.LastName;
        }
        if (dto.Roles != null)
        {
            entity.Roles = dto.Roles;
        }
    }
}
