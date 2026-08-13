using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Represents the database context for the Theme Park system.
/// It handles the database operations and mappings for entity configurations.
/// </summary>
internal class UCRDatabaseContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UCRDatabaseContext"/> class with specified options.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public UCRDatabaseContext(DbContextOptions<UCRDatabaseContext> options) : base()
    {
        _options = options;
    }

    private readonly DbContextOptions<UCRDatabaseContext>? _options;

    /// <summary>
    /// Configures the database connection and options when the context is being initialized.
    /// </summary>
    /// <param name="optionsBuilder">The builder to configure options.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && _options != null)
        {
            try
            {
                var extensions = _options.Extensions?.ToList();
                if (extensions != null)
                {
                    foreach (var extension in extensions)
                    {
                        ((Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                            .AddOrUpdateExtension(extension);
                    }
                }
            }
            catch
            {
                // Options are mocked or invalid, ignore
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UCRDatabaseContext"/> class.
    /// </summary>
    protected UCRDatabaseContext() : base()
    {
    }

    /// <summary>
    /// Gets or sets the collection of learning spaces in the database.
    /// </summary>
    public virtual DbSet<LearningSpace> LearningSpaces { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of learning components in the database.
    /// </summary>
    public virtual DbSet<LearningComponent> LearningComponents { get; set; } = null!;

    /// <summary>
    /// Configures the model relationships and entity mappings when the model for a context is being created.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply the LearningSpace entity configuration
        modelBuilder.ApplyConfiguration(new LearningSpaceEntityConfiguration());

        // Apply the LearningComponent entity configuration
        modelBuilder.ApplyConfiguration(new LearningComponentEntityConfiguration());

        // Alternatively, you can apply all configurations from the assembly
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
