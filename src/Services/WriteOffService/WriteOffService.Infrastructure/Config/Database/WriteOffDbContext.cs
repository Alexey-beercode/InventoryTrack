using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Entities;
using WriteOffService.Infrastructure.Config.Database.Configurations;
using WriteOffService.Infrastructure.Extensions;

namespace WriteOffService.Infrastructure.Config.Database;

public class WriteOffDbContext : DbContext
{
    public WriteOffDbContext(DbContextOptions<WriteOffDbContext> options) : base(options)
    {
        Database.Migrate();
    }
    
    public DbSet<Document> Documents { get; set; }
    public DbSet<WriteOffAct> WriteOffActs { get; set; }
    public DbSet<WriteOffReason> WriteOffReasons { get; set; }
    public DbSet<WriteOffRequest> WriteOffRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new WriteOffActConfiguration());
        modelBuilder.ApplyConfiguration(new WriteOffReasonConfiguration());
        modelBuilder.ApplyConfiguration(new WriteOffRequestConfiguration());
        modelBuilder.SeedWriteOffsReasonsData();
    }
}