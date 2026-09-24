using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for the <see cref="University"/> entity.
/// </summary>
internal class UniversityEntityConfiguration : IEntityTypeConfiguration<University>
{
    /// <summary>
    /// Configures the <see cref="University"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used for configuring the entity.</param>
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");

        builder.Property(u => u.Name)
            .HasColumnName("Name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Country)
            .HasColumnName("Country")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasKey(u => u.Name);
    }
}
