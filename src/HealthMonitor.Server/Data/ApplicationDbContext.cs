using Microsoft.EntityFrameworkCore;
using HealthMonitor.Shared.Models;

namespace HealthMonitor.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Sleep> SleepRecords => Set<Sleep>();
    public DbSet<HealthMetrics> HealthMetrics => Set<HealthMetrics>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Activity configuration
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(a => a.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Sleep configuration
        modelBuilder.Entity<Sleep>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(s => s.User)
                .WithMany(u => u.SleepRecords)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // HealthMetrics configuration
        modelBuilder.Entity<HealthMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}