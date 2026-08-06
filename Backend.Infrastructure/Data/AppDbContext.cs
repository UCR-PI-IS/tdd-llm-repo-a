using Microsoft.EntityFrameworkCore;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Data;

/// <summary>
/// Application database context.
/// This is an alias for UCRDatabaseContext to support test compatibility.
/// </summary>
internal class AppDbContext : UCRDatabaseContext
{
    /// <summary>
    /// Protected parameterless constructor for mocking support.
    /// </summary>
    protected AppDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public AppDbContext(DbContextOptions<UCRDatabaseContext> options) : base(options)
    {
    }
}
