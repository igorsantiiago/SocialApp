using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
    Task<AppUser> GetUserWithLikes(int userId);
    Task<PagedList<LikesDTO>> GetUserLikes(LikesParams likesParams);
}
