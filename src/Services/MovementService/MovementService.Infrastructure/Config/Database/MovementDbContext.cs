using Microsoft.EntityFrameworkCore;
using MovementService.Domain.Entities;
using MovementService.Infrastructure.Config.Database.Configurations;

namespace MovementService.Infrastructure.Config.Database;

public class MovementDbContext : DbContext
{
    public MovementDbContext(DbContextOptions<MovementDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<MovementRequest> MovementRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MovementRequestConfiguration());
    }
}