using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Config.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Config.Database;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
        Database.Migrate();
    }
    
    public DbSet<Document> Documents { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<InventoriesItemsWarehouses> InventoriesItemsWarehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new InventoryItemConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
        modelBuilder.ApplyConfiguration(new InventoriesItemsWarehousesConfiguration());
    }
}