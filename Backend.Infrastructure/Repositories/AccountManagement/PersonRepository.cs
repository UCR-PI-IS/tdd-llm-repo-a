using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Repository for managing persons.
/// This class implements the IPersonRepository interface.
/// </summary>
internal class PersonRepository : IPersonRepository
{
    /// <summary>
    /// Retrieves a person by their identity number from the real data source.
    /// (Not implemented yet – placeholder).
    /// </summary>
    /// <param name="identityNumber">The identity number of the person.</param>
    /// <returns>A person if found, otherwise null.</returns>
    public Task<Person?> GetByIdAsync(string identityNumber)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Creates a new person in the real data source.
    /// (Not implemented yet – placeholder).
    /// </summary>
    /// <param name="person"></param>
    /// <returns> True if the person was created successfully, otherwise false.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<bool> CreatePersonAsync(Person person)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Retrieves all persons from the real data source.
    /// </summary>
    /// <returns>A list of persons if found, otherwise an empty list.</returns>
    public Task<List<Person>> GetAllAsync()
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Deletes a person from the real data source.
    /// </summary>
    /// <param name="person"></param>
    /// <returns>
    /// A boolean indicating whether the deletion was successful.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<bool> DeletePersonAsync(string identityNumber)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

    /// <summary>
    /// Updates the details of an existing person in the real data source.
    /// </summary>
    /// <param name="identityNumber">The unique identity number of the person to be updated.</param>
    /// <param name="updatedPerson">The updated person object containing the new details.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the update was successful or not.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the method is not yet implemented.
    /// </exception>
    public Task<bool> UpdatePersonAsync(string identityNumber, Person updatedPerson)
    {
        throw new NotImplementedException("This method will be implemented when the real data source is integrated.");
    }

}
