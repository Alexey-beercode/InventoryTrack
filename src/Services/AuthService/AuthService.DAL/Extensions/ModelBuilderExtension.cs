using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.Extensions;

public static class ModelBuilderExtension
{
    public static void SeedUsersRolesData(this ModelBuilder modelBuilder)
    {
        var departmentHeadRole = new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Department Head"
        };
        var residentRole = new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Resident"
        };
        var warehouseManagerRole=new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Warehouse Manager"
        };
        var accountantRole=new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Accountant"
        };
        modelBuilder.Entity<Role>().HasData(residentRole);
        modelBuilder.Entity<Role>().HasData(departmentHeadRole);
        modelBuilder.Entity<Role>().HasData(warehouseManagerRole);
        modelBuilder.Entity<Role>().HasData(accountantRole);
    }
}