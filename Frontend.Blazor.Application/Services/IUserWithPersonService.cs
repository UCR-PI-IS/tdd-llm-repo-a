using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services;
/// <summary>
/// Defines the contract for user-person-related operations in the application.
/// This interface is intended to be implemented by services that handle user management functionality.
/// </summary>

public interface IUserWithPersonService
{
    /// <summary>
    /// Retrieves all users with associated personal information and their assigned roles.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of 
    /// <see cref="UserWithPerson"/> objects enriched with role information.
    /// </returns>
    Task<List<UserWithPerson>> GetAllUserWithPersonAsync();

    /// <summary>
    /// Creates a UserWithPerson in the database.
    /// </summary>
    /// <param name="userWithPerson">The user object containing the details of the userWithPerson to be created.</param>
    /// <returns> A task that represents the asynchronous operation. 
    /// The task result contains a boolean indicating success or failure.</returns>
    Task<bool> CreateUserWithPersonAsync(UserWithPerson userWithPerson);

    /// <summary>
    /// Deletes a user with person from the system by their user id and person id.
    /// </summary>
    /// <param name="personId">The unique identifier for the person associated with the user.</param>
    /// <param name="userId">The unique identifier for the user associated with the user.</param>
    /// <returns> A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the deletion was successful or not.</returns>
    Task<bool> DeleteUserWithPersonAsync(int userId, int personId);

    /// <summary>
    /// Updates a user with associated personal information in the database.
    /// </summary>
    /// <param name="user"> The user object containing the updated details of the user with person information.</param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task<bool> UpdateUserWithPersonAsync(UserWithPerson user);
}
