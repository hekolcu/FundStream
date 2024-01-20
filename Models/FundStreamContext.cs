using Microsoft.EntityFrameworkCore;

namespace FundStream.Models;

public class FundStreamContext: DbContext
{
    public FundStreamContext(DbContextOptions<FundStreamContext> options) : base(options)
    {
    }

    // Define DbSets for your entities, for example:
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Contribution> Contributions { get; set; }
    // Add additional DbSets for other entities

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Here you can configure the schema using Fluent API, for example:
        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasKey(u => u.UserId);

        modelBuilder.Entity<Project>()
            .ToTable("Projects")
            .HasKey(p => p.ProjectId);
        
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Contributions) // One-to-many relationship with Contributions
            .WithOne(c => c.Project) // Each Contribution is related to one Project
            .HasForeignKey(c => c.ProjectId); // Foreign key in the Contribution table
        
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Creator) // One-to-many relationship with User
            .WithMany(u => u.Projects) // Each User can have many Projects
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Contribution>()
            .ToTable("Contributions")
            .HasKey(c => c.ContributionId);
        
        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Contributor) // One-to-many relationship with User
            .WithMany(u => u.Contributions) // Each User can have many Contributions
            .HasForeignKey(c => c.UserId); // Foreign key in the Contribution table
                
        // Configure relationships and other constraints here
    }
}