﻿using DatingApp.API.Data;
using DatingApp.API.Data.Repositories;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configurations.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(configurations.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
