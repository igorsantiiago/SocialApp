using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsername(string username);
    Task<PagedList<AppUserDTO>> GetAppUsersDtoAsync(UserParams userParams);
    Task<AppUserDTO?> GetAppUserDtoByUsername(string username);
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
}
