using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="Person"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="Person"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id);

        builder.Property(p => p.FirstName)
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .HasMaxLength(100);

        builder.Property(p => p.Email)
            .HasMaxLength(255);

        builder.Property(p => p.IdentityNumber)
            .HasMaxLength(50);

        builder.Property(p => p.BirthDate);

        builder.Property(p => p.Phone)
            .HasMaxLength(50);
    }
}
