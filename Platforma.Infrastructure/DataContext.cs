using Microsoft.EntityFrameworkCore;
using Platforma.Domain;

namespace Platforma.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<CourseUser> CourseUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(cu => cu.Courses)
                .WithMany(cu => cu.Users)
                .UsingEntity<CourseUser>();
        }
    }
}
