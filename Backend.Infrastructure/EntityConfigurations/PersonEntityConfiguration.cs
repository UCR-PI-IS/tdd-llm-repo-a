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

        // Primary key configuration
        builder.HasKey(p => p.Id);

        // Property attribute configurations
        builder.Property(p => p.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.IdentityNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.BirthDate)
            .IsRequired();

        builder.Property(p => p.Phone)
            .HasMaxLength(50);
    }
}
