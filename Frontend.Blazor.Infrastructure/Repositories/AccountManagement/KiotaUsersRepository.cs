using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.AccountManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Mappers;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota.Models;
using Microsoft.Kiota.Abstractions;
namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Repository implementation for user and person data access using the Kiota-generated API client.
/// Provides methods to retrieve and create user-person combined entities from the backend API.
/// </summary>
internal class KiotaUsersRepository : IUserWithPersonRepository
{
    private readonly ApiClient _apiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="KiotaUsersRepository"/> class.
    /// </summary>
    /// <param name="apiClient">The Kiota-generated API client used for HTTP requests.</param>
    public KiotaUsersRepository(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Retrieves all users with their associated person information from the backend API.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a list of <see cref="UserWithPerson"/> entities.
    /// </returns>
    /// <exception cref="EntityNotFoundException">Thrown if no users are found.</exception>
    public async Task<List<UserWithPerson>> GetAllUserWithPersonAsync()
    {
        var response = await _apiClient.Userwithperson.GetAsync();

        if (response == null || !response.Any())
            return new List<UserWithPerson>();

        return response.Select(dto => dto.ToEntity()).ToList();
    }

    /// <summary>
    /// Creates a new user with associated person information in the backend API.
    /// </summary>
    /// <param name="user">The <see cref="UserWithPerson"/> entity containing user and person details to create.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is <c>true</c> if the creation was successful; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> CreateUserWithPersonAsync(UserWithPerson user)
    {
        var request = new PostCreateUserWithPersonRequest
        {
            UserWithPerson = new CreateUserWithPersonDto
            {
                UserName = user.UserName.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email.Value,
                Phone = user.Phone.Value,
                IdentityNumber = user.IdentityNumber.Value,
                // Documentation: https://learn.microsoft.com/en-us/openapi/kiota/abstractions
                BirthDate = new Microsoft.Kiota.Abstractions.Date(
                    user.BirthDate.Value.Year,
                    user.BirthDate.Value.Month,
                    user.BirthDate.Value.Day
                ),
                Roles = user.Roles
            }
        };

        var response = await _apiClient.Userwithperson.Create.PostAsync(request);
        return response is not null;
    }
    /// <summary>
    /// Deletes a user with associated person information from the backend API by their user ID and person ID.
    /// </summary>
    /// <param name="userId"> The unique identifier for the user to be deleted.</param>
    /// <param name="personId"> The unique identifier for the person associated with the user to be deleted.</param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public async Task<bool> DeleteUserWithPersonAsync(int userId, int personId)
    {
        var response = await _apiClient.Userwithperson.DeletePath.DeleteAsync(options =>
        {
            options.QueryParameters.UserId = userId;
            options.QueryParameters.PersonId = personId;
        });

        return response != null;
    }


    public async Task<bool> UpdateUserWithPersonAsync(UserWithPerson user)
    {
        var dto = new CreateUserWithPersonDto
        {
            UserName = user.UserName.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email.Value,
            Phone = user.Phone.Value,
            IdentityNumber = user.IdentityNumber.Value,
            BirthDate = user.BirthDate.Value,
            Roles = user.Roles
        };

        var request = new PutModifyUserWithPersonRequest
        {
            IdentityNumber = user.IdentityNumber.Value,
            UserWithPerson = dto
        };

        var result = await _apiClient.Userwithperson.Modify.PutAsync(request);
        return result is not null;
    }

}
