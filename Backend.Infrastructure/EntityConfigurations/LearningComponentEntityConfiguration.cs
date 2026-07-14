using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="LearningComponent"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class LearningComponentEntityConfiguration : IEntityTypeConfiguration<LearningComponent>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="LearningComponent"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LearningComponent> builder)
    {
        builder.ToTable("LearningComponent");

        // Primary key configuration
        builder.HasKey(component => component.ComponentId);

        // Property configurations
        builder.Property(component => component.ComponentId)
            .HasMaxLength(50);

        builder.Property(component => component.LearningSpaceId)
            .HasMaxLength(50);

        builder.Property(component => component.Orientation)
            .HasMaxLength(10);
    }
}
