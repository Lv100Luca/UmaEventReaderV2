using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Infrastructure;

using Microsoft.EntityFrameworkCore;

public class UmaDbContext : DbContext
{
    public DbSet<UmaEventEntity> Events => Set<UmaEventEntity>();
    public DbSet<UmaEventChoiceEntity> Choices => Set<UmaEventChoiceEntity>();
    public DbSet<UmaEventChoiceOutcomeEntity> Outcomes => Set<UmaEventChoiceOutcomeEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=35432;Database=uma_db;Username=umauser;Password=umapassword");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UmaEventChoiceOutcomeEntity>()
            .Property(o => o.Type)
            .HasConversion<string>(); // store enum as string instead of int

        base.OnModelCreating(modelBuilder);
    }
}
