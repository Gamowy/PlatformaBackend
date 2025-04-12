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
                .HasMany(x => x.Courses)
                .WithMany(x => x.Users)
                .UsingEntity<CourseUser>();

            modelBuilder.Entity<User>()
                .HasMany(x => x.OwnedCourses)
                .WithOne(x => x.Owner)
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Answers)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<Assignment>()
                .HasMany(x => x.Answers)
                .WithOne(x => x.Assignment)
                .HasForeignKey(x => x.AssignmentId)
                .HasPrincipalKey(x => x.Id);

            modelBuilder.Entity<Course>()
                .HasMany(x => x.Assignments)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
