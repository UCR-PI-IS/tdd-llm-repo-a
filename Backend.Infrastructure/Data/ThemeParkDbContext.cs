using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

public class ThemeParkDbContext : DbContext
{
    public ThemeParkDbContext(DbContextOptions<ThemeParkDbContext> options) : base(options) { }

    public DbSet<LearningComponent> LearningComponents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
