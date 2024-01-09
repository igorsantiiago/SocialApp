using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;

namespace DatingApp.API.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsername(string username);
    Task<IEnumerable<AppUserDTO>> GetAppUsersDtoAsync();
    Task<AppUserDTO?> GetAppUserDtoByUsername(string username);
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
}
