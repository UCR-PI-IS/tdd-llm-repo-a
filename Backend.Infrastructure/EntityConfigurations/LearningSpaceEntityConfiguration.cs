using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="LearningSpace"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class LearningSpaceEntityConfiguration : IEntityTypeConfiguration<LearningSpace>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="LearningSpace"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LearningSpace> builder)
    {
        builder.ToTable("LearningSpace");

        // Primary key configuration
        builder.HasKey(LearningSpace => LearningSpace.id);

        // Property atribute configurations
        builder.Property(LearningSpace => LearningSpace.type)
            .HasMaxLength(50);

        builder.Property(LearningSpace => LearningSpace.id)
            .HasMaxLength(50);

        // Configure to use parameterless constructor (if available) or skip constructor binding
        builder.Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
