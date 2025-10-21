using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Dummy implementation of the IPersonRepository using in-memory data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DummyPersonRepository"/> class.
/// </remarks>
/// <param name="logger">Logs information about the repository operations.</param>
internal class DummyPersonRepository(ILogger<DummyPersonRepository> logger) : IPersonRepository
{
    /// <summary>
    /// Logger for the DummyPersonRepository class.
    /// </summary>
    private readonly ILogger<DummyPersonRepository> _logger = logger;

    /// <summary>
    /// In-memory list of dummy people for testing purposes.
    /// </summary>
    private static readonly List<Person> _dummyPeople =
    [
        new Person(
            id: 1,
            email: Email.Create("andres.murillo@ucr.ac.cr"),
            firstName: "Andrés",
            lastName: "Murillo",
            phone: Phone.Create("8888-8888"),
            birthDate: BirthDate.Create(new DateOnly(2000, 05, 12)),
            identityNumber: IdentityNumber.Create("123")
        ),
        new Person(
            id: 2,
            email: Email.Create("gael.alpizar@ucr.ac.cr"),
            firstName: "Gael",
            lastName: "Alpizar",
            phone: Phone.Create("8777-7777"),
            birthDate: BirthDate.Create(new DateOnly(2001, 03, 20)),
            identityNumber: IdentityNumber.Create("456")
        ),
        new Person(
            id: 3,
            email: Email.Create("elizabeth.huang@ucr.ac.cr"),
            firstName: "Elizabeth",
            lastName: "Huang",
            phone: Phone.Create("8666-6666"),
            birthDate: BirthDate.Create(new DateOnly(2003, 07, 09)),
            identityNumber: IdentityNumber.Create("789")
        )
    ];

    /// <summary>
    /// Retrieves a person by their identity number.
    /// </summary>
    /// <param name="id">The identity number of the person.</param>
    /// <returns>The person if found, otherwise null.</returns>
    public Task<Person?> GetByIdAsync(string identityNumber)
    {
        var person = _dummyPeople.FirstOrDefault(p => p.IdentityNumber.Value == identityNumber);
        return Task.FromResult(person);
    }

    /// <summary>
    /// Creates a new person in the repository.
    /// </summary>
    /// <param name="person"> The person to create.</param>
    /// <returns> True if the person was created successfully, otherwise false.</returns>
    public async Task<bool> CreatePersonAsync(Person person)
    {
        _logger.LogInformation("Success in Person creation");
        await Task.Delay(TimeSpan.FromSeconds(1));
        return true;
    }

    /// <summary>
    /// Retrieves all persons from the dummy data source.   
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="Person"/> objects
    /// representing all persons in the dummy data source.
    /// </returns>
    public Task<List<Person>> GetAllAsync()
    {
        return Task.FromResult(_dummyPeople.ToList());
    }

    /// <summary>
    /// Deletes a person from the dummy data source.
    /// </summary>
    /// <param name="identityNumber"></param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating whether
    /// a person was deleted successfully.
    /// </returns>
    public Task<bool> DeletePersonAsync(string identityNumber)
    {
        var person = _dummyPeople.FirstOrDefault(p => p.IdentityNumber.Value == identityNumber);
        if (person != null)
        {
            _dummyPeople.Remove(person);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    /// <summary>
    /// Updates the details of an existing person in the dummy data source.
    /// </summary>
    /// <param name="identityNumber">The unique identity number of the person to update.</param>
    /// <param name="updatedPerson">An object containing the updated details of the person. 
    /// Fields that are <c>null</c>, <c>"string"</c>, or <c>default</c> will not overwrite existing values.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the person was successfully updated; 
    /// otherwise, <c>false</c> if the person with the specified identity number was not found.
    /// </returns>
    public Task<bool> UpdatePersonAsync(string identityNumber, Person updatedPerson)
    {
        var person = _dummyPeople.FirstOrDefault(p => p.IdentityNumber.Value == identityNumber);
        if (person == null)
        {
            return Task.FromResult(false);
        }

        if (updatedPerson.FirstName != "string" && !string.IsNullOrWhiteSpace(updatedPerson.FirstName))
        {
            person.FirstName = updatedPerson.FirstName;
        }

        if (updatedPerson.LastName != "string" && !string.IsNullOrWhiteSpace(updatedPerson.LastName))
        {
            person.LastName = updatedPerson.LastName;
        }

        if (updatedPerson.Email.Value != "string" && !string.IsNullOrWhiteSpace(updatedPerson.Email.Value))
        {
            person.Email = updatedPerson.Email;
        }

        if (updatedPerson.Phone.Value != "string" && !string.IsNullOrWhiteSpace(updatedPerson.Phone.Value))
        {
            person.Phone = updatedPerson.Phone;
        }

        if (updatedPerson.BirthDate != default && updatedPerson.BirthDate.Value != DateOnly.FromDateTime(DateTime.Now))
        {
            person.BirthDate = updatedPerson.BirthDate;
        }

        return Task.FromResult(true);
    }

}