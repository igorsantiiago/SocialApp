﻿using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequiredLength = 12;
        }).AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<AppDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            opt.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        context.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Admin", "Moderador"));
            options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
        });

        return services;
    }
}
