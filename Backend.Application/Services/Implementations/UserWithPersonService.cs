using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Provides implementation for user-with-person-related operations defined in <see cref="IUserWithPersonService"/>.
/// </summary>
internal class UserWithPersonService : IUserWithPersonService
{
    private readonly IUserWithPersonRepository _userWithPersonRepository;
    private readonly IUserRoleService _userRoleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserWithPersonService"/> class.
    /// </summary>
    /// <param name="userWithPersonRepository">The repository used to access user and person data.</param>
    public UserWithPersonService(
      IUserWithPersonRepository userWithPersonRepository,
      IUserRoleService userRoleService)
    {
        _userWithPersonRepository = userWithPersonRepository;
        _userRoleService = userRoleService;
    }

    /// <summary>
    /// Retrieves a list of all users with people in the database.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="UserWithPerson"/> objects
    /// if found, or an empty list if none exist in the database.
    /// </returns>
    public Task<List<UserWithPerson>> GetAllUserWithPersonAsync()
    {
        return _userWithPersonRepository.GetAllUserWithPersonAsync();
    }


    /// <summary>
    /// Creates a new userWithPerson in the system.
    /// </summary>
    /// <param name="userWithPerson"> The user object containing the details of the userWithPerson to be created.</param>
    /// <returns> A task that represents the asynchronous operation. 
    /// The task result contains a boolean indicating success or failure.</returns>
    public Task<bool> CreateUserWithPersonAsync(UserWithPerson userWithPerson)
    {
        return _userWithPersonRepository.CreateUserWithPersonAsync(userWithPerson);
    }

    /// <summary>
    /// Deletes a user with person from the system by their user id and person id.
    /// </summary>
    /// <param name="personId">The unique identifier for the person associated with the user.</param>
    /// <param name="userId">The unique identifier for the user associated with the user.</param>
    /// <returns> A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the deletion was successful or not.</returns>
    public Task<bool> DeleteUserWithPersonAsync(int userId, int personId)
    {
        return _userWithPersonRepository.DeleteUserWithPersonAsync(userId, personId);
    }

    /// <summary>
    /// Updates a user with associated personal information in the database.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<bool> UpdateUserWithPersonAsync(UserWithPerson user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }
        // Validate the user object here if necessary
        return _userWithPersonRepository.UpdateUserWithPersonAsync(user);
    }
}
