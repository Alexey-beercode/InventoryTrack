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
            Name = "Начальник подразделения"
        };
        var residentRole = new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Пользователь"
        };
        var warehouseManagerRole=new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Материально-ответственное лицо"
        };
        var accountantRole=new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Бухгалтер"
        };
        modelBuilder.Entity<Role>().HasData(residentRole);
        modelBuilder.Entity<Role>().HasData(departmentHeadRole);
        modelBuilder.Entity<Role>().HasData(warehouseManagerRole);
        modelBuilder.Entity<Role>().HasData(accountantRole);
    }
}