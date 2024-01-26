using DatingApp.API.Entities;

namespace DatingApp.API;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}
