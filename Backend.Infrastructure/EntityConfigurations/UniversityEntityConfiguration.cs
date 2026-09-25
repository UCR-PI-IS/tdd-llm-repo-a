using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="University"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class UniversityEntityConfiguration : IEntityTypeConfiguration<University>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="University"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("University");

        builder.HasKey(u => u.UniversityId);

        builder.Property(u => u.UniversityId);

        builder.Property(u => u.Name)
            .HasMaxLength(200);

        builder.Property(u => u.Country)
            .HasMaxLength(100);
    }
}
