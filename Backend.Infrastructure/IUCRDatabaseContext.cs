using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Interface for the database context to enable testability.
/// </summary>
public interface IUCRDatabaseContext
{
    /// <summary>
    /// Gets or sets the collection of learning spaces in the database.
    /// </summary>
    DbSet<LearningSpace> LearningSpaces { get; set; }

    /// <summary>
    /// Gets or sets the collection of learning components in the database.
    /// </summary>
    DbSet<LearningComponent> LearningComponents { get; set; }
}
