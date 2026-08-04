using LibraryManagement.Api.Constants;
using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Api.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager =
            serviceProvider.GetRequiredService<
                RoleManager<IdentityRole>>();

        var userManager =
            serviceProvider.GetRequiredService<
                UserManager<ApplicationUser>>();

        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        await CreateRoleAsync(roleManager, UserRoles.Student);
        await CreateRoleAsync(roleManager, UserRoles.Admin);

        var adminEmail = configuration["AdminSeed:Email"];
        var adminPassword = configuration["AdminSeed:Password"];
        var adminFirstName =
            configuration["AdminSeed:FirstName"] ?? "Library";
        var adminLastName =
            configuration["AdminSeed:LastName"] ?? "Admin";

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException(
                "Admin başlangıç bilgileri User Secrets içinde bulunamadı.");
        }

        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                FirstName = adminFirstName,
                LastName = adminLastName,
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true
            };

            var createResult =
                await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Admin kullanıcısı oluşturulamadı: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(
                adminUser,
                UserRoles.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                UserRoles.Admin);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Admin rolü eklenemedi: {errors}");
            }
        }
    }

    private static async Task CreateRoleAsync(
        RoleManager<IdentityRole> roleManager,
        string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(
            new IdentityRole(roleName));

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"{roleName} rolü oluşturulamadı: {errors}");
        }
    }
}