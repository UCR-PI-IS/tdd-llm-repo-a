using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="Building"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class BuildingEntityConfiguration : IEntityTypeConfiguration<Building>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="Building"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Building");

        builder.HasKey(b => b.InternalId);

        builder.Property(b => b.InternalId);

        builder.Property(b => b.Name)
            .HasMaxLength(100);

        builder.Property(b => b.Color)
            .HasMaxLength(50);

        builder.Property(b => b.Height);
        builder.Property(b => b.Length);
        builder.Property(b => b.Width);
        builder.Property(b => b.X);
        builder.Property(b => b.Y);
        builder.Property(b => b.Z);

        builder.Property(b => b.AreaId);
    }
}
