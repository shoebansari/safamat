using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
        await SeedAdminUsersAsync(context);
    }

    private static async Task SeedAdminUsersAsync(ApplicationDbContext context)
    {
        var seedUsers = new[]
        {
            new
            {
                AdminUserName = "admin",
                Password = "Admin@123",
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@matrimonial.com",
                Phone = (string?)null
            },
            new
            {
                AdminUserName = "SHOEB",
                Password = "12345",
                FirstName = "shoeb",
                LastName = "ansari",
                Email = "s@gmail.com",
                Phone = (string?)"123123"
            }
        };

        foreach (var seed in seedUsers)
        {
            if (await context.AdminUsers.AnyAsync(u => u.AdminUserName == seed.AdminUserName))
                continue;

            context.AdminUsers.Add(new AdminUser
            {
                AdminId = Guid.NewGuid(),
                AdminUserName = seed.AdminUserName,
                Password = BCrypt.Net.BCrypt.HashPassword(seed.Password),
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                Email = seed.Email,
                Phone = seed.Phone,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}
