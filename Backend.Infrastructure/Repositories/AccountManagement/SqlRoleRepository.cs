using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Implementation of <see cref="IRoleRepository"/> that interacts with the SQL database using EF Core.
/// </summary>
internal class SqlRoleRepository : IRoleRepository
{
    private readonly ThemeParkDataBaseContext _dbContext;
    private readonly ILogger<SqlRoleRepository> _logger;

    /// <summary>
    /// Constructor for <see cref="SqlRoleRepository"/>.
    /// </summary>
    /// <param name="dbContext">Injected database context for accessing data.</param>
    /// <param name="logger">Logger for recording operational messages and errors.</param>
    public SqlRoleRepository(ThemeParkDataBaseContext dbContext, ILogger<SqlRoleRepository> logger)
        => (_dbContext, _logger) = (dbContext, logger);

    /// <summary>
    /// Removes a role from the database.
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteRoleAsync(int roleId)
    {
        try
        {
            var existing = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId);
            if (existing == null)
            {
                _logger.LogWarning("Role not found for RoleId {RoleId}", roleId);
                return false;
            }

            // Remove the person entity
            _dbContext.Roles.Remove(existing);

            // Save changes to the database
            var result = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Role with Role Id {roleId} deleted.", roleId);
            return result > 0;
        }
        catch (DbUpdateConcurrencyException concurrencyEx) // Handle concurrency issues
        {
            _logger.LogError(concurrencyEx, "Concurrency error while deleting role with Role Id {roleId}", roleId);
            return false;
        }
        catch (DbUpdateException dbEx) // Handle database update issues
        {
            _logger.LogError(dbEx, "Database update error while deleting role with Role Id {roleId}", roleId);
            return false;
        }
        catch (Exception ex) // Handle any other unexpected errors
        {
            _logger.LogError(ex, "Unexpected error while deleting role with Role Id {roleId}", roleId);
            return false;
        }

    }

    /// <summary>
    /// Retrieves all roles registered in the database.
    /// </summary>
    /// <returns>A list of <see cref="Role"/> entries; empty list if an error occurs.</returns>
    public async Task<List<Role>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _dbContext.Roles.ToListAsync();
            _logger.LogInformation("Retrieved {Count} roles from database", roles.Count);
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles from database");
            // This is functionally the same as: return new List<Roles>()
            return [];
        }
    }

}
