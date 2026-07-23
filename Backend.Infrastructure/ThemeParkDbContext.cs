using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure
{
    public class ThemeParkDbContext : DbContext
    {
        public ThemeParkDbContext(DbContextOptions<ThemeParkDbContext> options) : base(options) { }
        public ThemeParkDbContext() { }

        public virtual DbSet<LearningComponent> LearningComponents { get; set; } = null!;
    }
}