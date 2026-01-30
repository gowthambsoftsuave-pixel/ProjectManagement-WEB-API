using Microsoft.EntityFrameworkCore;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.DAL.Data
{
    public class ProjectManagementDbContext : DbContext
    {
        public ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Person> Persons { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ ProjectTask → Assigned Person
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedToPerson)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.AssignedToPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ProjectTask → Project
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ ProjectTeam → Project + Person (many-to-many)
            modelBuilder.Entity<ProjectTeam>()
                .HasIndex(pt => new { pt.ProjectId, pt.PersonId })
                .IsUnique();

            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTeam)
                .HasForeignKey(pt => pt.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Person)
                .WithMany(p => p.ProjectTeams)
                .HasForeignKey(pt => pt.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Project → CreatedByAdmin
            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedByAdmin)
                .WithMany()
                .HasForeignKey(p => p.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
