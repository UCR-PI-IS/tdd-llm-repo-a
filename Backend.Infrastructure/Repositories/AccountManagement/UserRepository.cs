using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Repository for managing users.
/// This class implements the IUserRepository interface.
/// </summary>
internal class UserRepository : IUserRepository
{
    /// <summary>
    /// Deletes a user from the real data source.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// A boolean indicating whether the deletion was successful.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<bool> DeleteUserAsync(int id)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Creates a new user in the real data source.
    /// (Not implemented yet – placeholder).
    /// </summary>
    /// <param name="user"></param>
    /// <returns> True if the user was created successfully, otherwise false.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<bool> CreateUserAsync(User user)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Retrieves all users from the real data source.
    /// </summary>
    /// <returns>A list of users if found, otherwise an empty list.</returns>
    public Task<List<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Retrieves a user by their ID from the real data source.
    /// </summary>
    /// <param name="id"></param> The id of the user to be retrieved.
    /// <param name="user"></param> The user object containing the updated details of the user.
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<bool> ModifyUserAsync(int id, User user)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Retrieves a user by their ID from the real data source.
    /// </summary>
    /// <param name="id"></param> The id of the user to be retrieved.
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<User?> GetUserByIdAsync(int id)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }


}
