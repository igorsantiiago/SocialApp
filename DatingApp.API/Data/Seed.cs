﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data;

public class Seed
{
    public static async Task ClearConnections(AppDbContext context)
    {
        context.Connections.RemoveRange(context.Connections);
        await context.SaveChangesAsync();
    }

    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync())
            return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        if (users == null)
            return;

        var roles = new List<AppRole>{
            new() {Name="Admin"},
            new() {Name="Moderador"},
            new() {Name="Usuário"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {
            user.Photos.First().IsApproved = true;
            user.UserName = user.UserName!.ToLower();
            user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
            user.LastActivity = DateTime.SpecifyKind(user.LastActivity, DateTimeKind.Utc);
            await userManager.CreateAsync(user, "Pa$$w0rd1234");
            await userManager.AddToRoleAsync(user, "Usuário");
        }

        var admin = new AppUser { UserName = "admin" };
        await userManager.CreateAsync(admin, "Pa$$w0rd1234");
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderador" });
    }
}
