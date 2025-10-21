using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Implementation of <see cref="IPersonRepository"/> that interacts with the SQL database using EF Core.
/// </summary>
internal class SqlPersonRepository : IPersonRepository
{
    private readonly ThemeParkDataBaseContext _dbContext;
    private readonly ILogger<SqlPersonRepository> _logger;

    /// <summary>
    /// Constructor for <see cref="SqlPersonRepository"/>.
    /// </summary>
    /// <param name="dbContext">Injected database context for accessing data.</param>
    /// <param name="logger">Logger for recording operational messages and errors.</param>
    public SqlPersonRepository(ThemeParkDataBaseContext dbContext, ILogger<SqlPersonRepository> logger)
    => (_dbContext, _logger) = (dbContext, logger);

    /// <summary>
    /// Creates a new <see cref="Person"/> entry in the database.
    /// </summary>
    /// <param name="person">The person entity to insert.</param>
    /// <returns>True if creation was successful; false otherwise.</returns>
    public async Task<bool> CreatePersonAsync(Person person)
    {
        try
        {
            bool exists = await _dbContext.Persons.AnyAsync(p =>
              p.IdentityNumber == person.IdentityNumber ||
              p.Email == person.Email);


            if (exists)
            {
                _logger.LogWarning("Attempt to create person with existing identity number or email: {IdentityNumber}, {Email}", person.IdentityNumber, person.Email);
                return false;
            }

            // If the person does not exist, add it to the database
            _dbContext.Persons.Add(person);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully created person with Id: {Id}", person.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            return false;
        }
    }

    /// <summary>
    /// Retrieves a person by their identity number.
    /// </summary>
    /// <param name="identityNumber">The identity number to search for.</param>
    /// <returns>The matched <see cref="Person"/> or null if not found.</returns>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public async Task<Person?> GetByIdAsync(string identityNumber)
    {
        try
        {
            var person = await _dbContext.Persons
                .FirstOrDefaultAsync(p => p.IdentityNumber == IdentityNumber.Create(identityNumber));


            if (person is null)
            {
                _logger.LogWarning("Person with identity number {IdentityNumber} not found.", identityNumber);
            }
            else
            {
                _logger.LogInformation("Person with identity number {IdentityNumber} retrieved.", identityNumber);
            }

            return person;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving person with identity number {IdentityNumber}", identityNumber);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all people registered in the database.
    /// </summary>
    /// <returns>A list of <see cref="Person"/> entries; empty list if an error occurs.</returns>
    public async Task<List<Person>> GetAllAsync()
    {
        try
        {
            var people = await _dbContext.Persons.ToListAsync();
            _logger.LogInformation("Retrieved {Count} people from database", people.Count);
            return people;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving people from database");
            // This is functionally the same as: return new List<Person>()
            return [];
        }
    }

    /// <summary>
    /// Deletes a person from the database by their identity number.
    /// </summary>
    /// <param name="identityNumber"></param>
    /// <returns>
    /// A boolean indicating whether the deletion was successful.
    /// </returns>
    public async Task<bool> DeletePersonAsync(string identityNumber)
    {
        try
        {
            var person = await _dbContext.Persons
                .FirstOrDefaultAsync(p => p.IdentityNumber == IdentityNumber.Create(identityNumber));

            if (person is null)
            {
                _logger.LogWarning("Person with identity number {IdentityNumber} not found.", identityNumber);
                return false;
            }

            // Remove the person entity
            _dbContext.Persons.Remove(person);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Person with identity number {IdentityNumber} deleted.", identityNumber);
            return true;
        }
        catch (DbUpdateConcurrencyException concurrencyEx) // Handle concurrency issues
        {
            _logger.LogError(concurrencyEx, "Concurrency error while deleting person with identity number {IdentityNumber}", identityNumber);
            return false;
        }
        catch (DbUpdateException dbEx) // Handle database update issues
        {
            _logger.LogError(dbEx, "Database update error while deleting person with identity number {IdentityNumber}", identityNumber);
            return false;
        }
        catch (Exception ex) // Handle any other unexpected errors
        {
            _logger.LogError(ex, "Unexpected error while deleting person with identity number {IdentityNumber}", identityNumber);
            return false;
        }
    }

    /// <summary>
    /// Updates the details of an existing person in the database.
    /// </summary>
    /// <param name="identityNumber">
    /// The unique identity number of the person to be updated.
    /// </param>
    /// <param name="updatedPerson">
    /// A <see cref="Person"/> object containing the updated details of the person.
    /// Only non-default and non-empty values will be applied to the existing record.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the update was successful (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown if an unexpected error occurs during the update process.
    /// </exception>
    public async Task<bool> UpdatePersonAsync(string identityNumber, Person updatedPerson)
    {
        try
        {
            var existingPerson = await _dbContext.Persons
                .FirstOrDefaultAsync(p => p.IdentityNumber == IdentityNumber.Create(identityNumber));

            if (existingPerson == null)
            {
                _logger.LogWarning("Person with IdentityNumber {IdentityNumber} not found.", identityNumber);
                return false;
            }

            if (updatedPerson.Email.Value != "string" && !string.IsNullOrWhiteSpace(updatedPerson.Email.Value))
            {
                existingPerson.Email = updatedPerson.Email;
                // existingPerson.Email = Email.Create(updatedPerson.Email);
            }

            if (updatedPerson.FirstName != "string" && !string.IsNullOrWhiteSpace(updatedPerson.FirstName))
            {
                existingPerson.FirstName = updatedPerson.FirstName;
            }

            if (updatedPerson.LastName != "string" && !string.IsNullOrWhiteSpace(updatedPerson.LastName))
            {
                existingPerson.LastName = updatedPerson.LastName;
            }

            if (updatedPerson.Phone.Value != "string" && !string.IsNullOrWhiteSpace(updatedPerson.Phone.Value))
            {
                existingPerson.Phone = updatedPerson.Phone;
            }

            if (updatedPerson.BirthDate != default && updatedPerson.BirthDate.Value != DateOnly.FromDateTime(DateTime.Now))
            {
                existingPerson.BirthDate = updatedPerson.BirthDate;
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Person with IdentityNumber {IdentityNumber} successfully updated.", identityNumber);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person with IdentityNumber {IdentityNumber}.", identityNumber);
            return false;
        }
    }
}
