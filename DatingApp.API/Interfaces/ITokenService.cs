using DatingApp.API.Entities;

namespace DatingApp.API;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
