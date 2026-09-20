using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="Whiteboard"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// Uses TPH (Table Per Hierarchy) inheritance with LearningComponent.
/// </summary>
internal class WhiteboardEntityConfiguration : IEntityTypeConfiguration<Whiteboard>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="Whiteboard"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Whiteboard> builder)
    {
        // TPH inheritance - Whiteboard shares the LearningComponent table
        // The Discriminator column distinguishes between types
        builder.HasBaseType<LearningComponent>();

        // Configure Whiteboard-specific property
        builder.Property(w => w.MarkerColor)
            .HasMaxLength(50);
    }
}
