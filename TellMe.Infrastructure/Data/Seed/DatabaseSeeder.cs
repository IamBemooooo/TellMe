using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Core.Entities;
using TellMe.Data;
using TellMe.Infrastructure.Services;

namespace TellMe.Infrastructure.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<AppDbContext>();

            // Apply pending migrations (preferred). If migrations are not available
            // fallback to EnsureCreated to create schema for quick development.
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception)
            {
                // If migrations are not present or there is an issue applying them,
                // try EnsureCreated as a fallback so seeding can proceed in dev.
                await context.Database.EnsureCreatedAsync();
            }

            var hasher = provider.GetRequiredService<IPasswordHasher>();

            var now = DateTime.UtcNow;

            // 1) Permissions (5 quyền cố định)
            var permissionsToEnsure = new[]
            {
                new Permission { Id = Guid.NewGuid(), Name = "System.Manage", Description = "Quản trị hệ thống", Resource = "System", Action = "Manage", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = "Case.Process", Description = "Tiếp nhận và xử lý hồ sơ", Resource = "Case", Action = "Process", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = "News.Manage", Description = "Quản lý tin tức", Resource = "News", Action = "Manage", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = "UserHistory.View", Description = "Xem lịch sử người dùng", Resource = "UserHistory", Action = "View", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = "Case.Complete", Description = "Trả kết quả và xử lý hồ sơ", Resource = "Case", Action = "Complete", CreatedAt = now }
            };

            foreach (var p in permissionsToEnsure)
            {
                var exists = await context.Permissions
                    .FirstOrDefaultAsync(x => x.Resource == p.Resource && x.Action == p.Action);

                if (exists == null)
                {
                    context.Permissions.Add(p);
                }
                else
                {
                    // Update fields but keep existing Id
                    exists.Name = p.Name;
                    exists.Description = p.Description;
                    exists.Resource = p.Resource;
                    exists.Action = p.Action;
                    exists.IsActive = p.IsActive;
                    exists.UpdatedAt = now;
                    context.Permissions.Update(exists);
                }
            }

            await context.SaveChangesAsync();

            // 2) Role admin (create or update)
            var adminRoleName = "Admin";
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == adminRoleName);
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = adminRoleName,
                    Description = "Administrator role with all permissions",
                    CreatedAt = now,
                    IsActive = true
                };
                context.Roles.Add(adminRole);
            }
            else
            {
                adminRole.Description = "Administrator role with all permissions";
                adminRole.IsActive = true;
                adminRole.UpdatedAt = now;
                context.Roles.Update(adminRole);
            }

            await context.SaveChangesAsync();

            // 3) Assign all permissions to admin role (add missing only)
            var allPermissions = await context.Permissions.ToListAsync();
            foreach (var perm in allPermissions)
            {
                var exists = await context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == perm.Id);

                if (!exists)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = adminRole.Id,
                        PermissionId = perm.Id,
                        CreatedAt = now
                    });
                }
            }

            await context.SaveChangesAsync();

            // 4) Create or update admin user
            var adminUsername = "admin";
            var adminEmail = "admin@local";

            var adminUser = await context.Users
                .FirstOrDefaultAsync(u => u.Username == adminUsername || u.Email == adminEmail);

            if (adminUser == null)
            {
                var rawPassword = "Admin@123";
                var passwordHash = hasher.HashPassword(rawPassword);

                adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = adminUsername,
                    Email = adminEmail,
                    PasswordHash = passwordHash,
                    Name = "System Administrator",
                    IsActive = true,
                    CreatedAt = now,
                };

                context.Users.Add(adminUser);
            }
            else
            {
                // Update fields but do not overwrite password unless empty
                adminUser.Email = adminEmail;
                adminUser.Name = adminUser.Name ?? "System Administrator";
                adminUser.IsActive = true;
                adminUser.UpdatedAt = now;
                if (string.IsNullOrWhiteSpace(adminUser.PasswordHash))
                {
                    var rawPassword = "Admin@123";
                    adminUser.PasswordHash = hasher.HashPassword(rawPassword);
                }
                context.Users.Update(adminUser);
            }

            await context.SaveChangesAsync();

            // 5) Ensure admin user has admin role (user-role link)
            var hasUserRole = await context.UserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
            if (!hasUserRole)
            {
                context.UserRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    CreatedAt = now
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
