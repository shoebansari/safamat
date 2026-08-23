using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, bool includeDemoData = false)
    {
        await context.Database.MigrateAsync();
        await SeedAdminUsersAsync(context);

        if (!includeDemoData)
            return;

        await SeedDemoMembersAsync(context);
        await SeedDemoUsersAsync(context);
        await DbSeederDiscover.SeedBulkDiscoverUsersAsync(context);
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

    private static async Task SeedDemoMembersAsync(ApplicationDbContext context)
    {
        if (await context.Members.AnyAsync()) return;

        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.IsActive);
        if (tenant == null) return;

        context.Members.AddRange(
            new Member
            {
                MemberId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                UserCode = "USR001",
                FullName = "Rahul Sharma",
                Email = "rahul@example.com",
                Phone = "9876543210",
                Bio = "Software engineer, Mumbai",
                ProfilePhotoUrl = "https://placehold.co/200x200?text=Rahul",
                ProfileStatus = "Pending",
                PhotoStatus = "Pending",
                CreatedOn = DateTime.UtcNow
            },
            new Member
            {
                MemberId = Guid.NewGuid(),
                TenantId = tenant.TenantId,
                UserCode = "USR002",
                FullName = "Priya Patel",
                Email = "priya@example.com",
                Phone = "9876543211",
                Bio = "Doctor, Ahmedabad",
                ProfilePhotoUrl = "https://placehold.co/200x200?text=Priya",
                ProfileStatus = "Approved",
                PhotoStatus = "Pending",
                CreatedOn = DateTime.UtcNow
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedDemoUsersAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.IsActive);
        if (tenant == null) return;

        var rahulId = Guid.NewGuid();
        var priyaId = Guid.NewGuid();

        context.Users.AddRange(
            new User
            {
                UserId = rahulId,
                TenantId = tenant.TenantId,
                UserCode = "USR001",
                UserName = "rahul",
                FirstName = "Rahul",
                LastName = "Sharma",
                Email = "rahul@example.com",
                Phone = "9876543210",
                Password = BCrypt.Net.BCrypt.HashPassword("User@123"),
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                Profile = new UserProfile
                {
                    ProfileId = Guid.NewGuid(),
                    UserId = rahulId,
                    Gender = "Male",
                    DateOfBirth = new DateOnly(1995, 5, 15),
                    Height = 175,
                    Religion = "Hindu",
                    AboutMe = "Software engineer from Mumbai",
                    IsProfileCompleted = true,
                    ProfileStatus = "Pending",
                    CreatedOn = DateTime.UtcNow
                }
            },
            new User
            {
                UserId = priyaId,
                TenantId = tenant.TenantId,
                UserCode = "USR002",
                UserName = "priya",
                FirstName = "Priya",
                LastName = "Patel",
                Email = "priya@example.com",
                Phone = "9876543211",
                Password = BCrypt.Net.BCrypt.HashPassword("User@123"),
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                Profile = new UserProfile
                {
                    ProfileId = Guid.NewGuid(),
                    UserId = priyaId,
                    Gender = "Female",
                    DateOfBirth = new DateOnly(1997, 8, 20),
                    Height = 162,
                    Religion = "Hindu",
                    AboutMe = "Doctor from Ahmedabad",
                    IsProfileCompleted = true,
                    ProfileStatus = "Approved",
                    CreatedOn = DateTime.UtcNow
                }
            });

        context.UserPhotos.AddRange(
            new UserPhoto
            {
                PhotoId = Guid.NewGuid(), UserId = rahulId,
                PhotoUrl = "https://placehold.co/200x200?text=Rahul",
                IsPrimary = true, DisplayOrder = 1, IsApproved = false
            },
            new UserPhoto
            {
                PhotoId = Guid.NewGuid(), UserId = priyaId,
                PhotoUrl = "https://placehold.co/200x200?text=Priya",
                IsPrimary = true, DisplayOrder = 1, IsApproved = true
            });

        context.UserLocations.AddRange(
            new UserLocation { LocationId = Guid.NewGuid(), UserId = rahulId, Country = "India", State = "Maharashtra", City = "Mumbai" },
            new UserLocation { LocationId = Guid.NewGuid(), UserId = priyaId, Country = "India", State = "Gujarat", City = "Ahmedabad" });

        context.UserOccupations.AddRange(
            new UserOccupation { OccupationId = Guid.NewGuid(), UserId = rahulId, Occupation = "Software Engineer", WorkLocation = "Mumbai" },
            new UserOccupation { OccupationId = Guid.NewGuid(), UserId = priyaId, Occupation = "Doctor", WorkLocation = "Ahmedabad" });

        await context.SaveChangesAsync();
    }
}
