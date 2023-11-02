using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Domain.DataContext;
public class AppDbContext : DbContext
{
    public DbSet<PollingStationModel> PollingStations { get; set; }
    public DbSet<TagModel> Tags { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PollingStationModel>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.PollingStations)
            .UsingEntity<PollingStationTag>(
            l => l.HasOne<TagModel>().WithMany(e => e.PollingStationTags).HasForeignKey("TagId"),
            r => r.HasOne<PollingStationModel>().WithMany(e => e.PollingStationTags).HasForeignKey("PollingStationId"),
            j => j.HasKey("PollingStationId", "TagId")
            )
            .Navigation(e => e.Tags).AutoInclude();

        modelBuilder.Entity<TagModel>()
           .HasMany(p => p.PollingStationTags)
           .WithOne(t => t.Tag).HasForeignKey("TagId");
    }
}
