using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.AccountManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.AccountManagement;

/// <summary>
/// Repository implementation for managing role-permission associations using a SQL database.
/// Provides methods to retrieve permissions assigned to specific roles.
/// </summary>
internal class SqlRolePermissionRepository : IRolePermissionRepository
{
    private readonly ThemeParkDataBaseContext _dbContext;
    private readonly ILogger<SqlRolePermissionRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlRolePermissionRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing role and permission data.</param>
    /// <param name="logger">The logger instance for logging errors and information.</param>
    public SqlRolePermissionRepository(ThemeParkDataBaseContext dbContext, ILogger<SqlRolePermissionRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the list of permissions associated with the specified role identifier.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="Permission"/> objects
    /// assigned to the role, or <c>null</c> if the role does not exist or has no permissions.
    /// </returns>
    public async Task<List<Permission>?> ViewPermissionsByRoleIdAsync(int roleId)
    {
        try
        {
            var permissions = await _dbContext.Permissions
                .Include(x => x.RolePermissions)
                .Where(x => x.RolePermissions.Any(x => x.RoleId == roleId))
                .ToListAsync();
            return permissions!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for role with ID {RoleId}", roleId);
            return null;
        }
    }

    /// <summary>
    /// Assigns a permission to a role.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="permId"></param>
    /// <returns></returns>
    public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permId)
    {
        // Check if the user-role association already exists
        var exists = await _dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permId);
        if (exists)
        {
            _logger.LogWarning("Permission already assigned: RoleID {RoleId} and PermId {PermId}", roleId, permId);
            return false;
        }

        // Check if role exists
        var roleExists = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId);
        if (roleExists is null)
        {
            _logger.LogWarning("Role not found for RoleId {RoleId}", roleId);
            return false;
        }

        // Check if permission exists
        var permExists = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == permId);
        if (permExists is null)
        {
            _logger.LogWarning("Permission not found for PermissionId {PermId}", permId);
            return false;
        }

        var rolePerm = new RolePermission(roleId, permId);
        _dbContext.RolePermissions.Add(rolePerm);
        var result = await _dbContext.SaveChangesAsync();

        // Returns 1 if at least 1 role was affected
        return result > 0;
    }

    /// <summary>
    /// Removes a permission from a role.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="permId"></param>
    /// <returns></returns>
    public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permId)
    {
        // Check if the user-role association already exists
        var exists = await _dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permId);
        if (!exists)
        {
            _logger.LogWarning("Permission not assigned: RoleID {RoleId} and PermId {PermId}", roleId, permId);
            return false;
        }

        // Check if role exists
        var roleExists = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId);
        if (roleExists is null)
        {
            _logger.LogWarning("Role not found for RoleId {RoleId}", roleId);
            return false;
        }

        // Check if permission exists
        var permExists = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == permId);
        if (permExists is null)
        {
            _logger.LogWarning("Permission not found for PermissionId {PermId}", permId);
            return false;
        }

        var rolePermission = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permId);

        if (rolePermission is null)
        {
            _logger.LogWarning("Error removing permission from role");

            return false;
        }
        else
        {
            _dbContext.RolePermissions.Remove(rolePermission);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

    }
}
