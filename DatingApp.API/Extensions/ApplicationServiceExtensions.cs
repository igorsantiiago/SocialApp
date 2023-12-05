using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlite(configurations.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
