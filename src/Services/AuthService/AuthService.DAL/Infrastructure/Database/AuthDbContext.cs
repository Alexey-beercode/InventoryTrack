using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.Infrastructure.Database;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
    }
}