using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data.Repositories;

public class LikesRepository : ILikesRepository
{
    private readonly AppDbContext _context;
    public LikesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        var likes = await _context.UserLikes.FindAsync(sourceUserId, targetUserId);
        return likes!;
    }

    public async Task<PagedList<LikesDTO>> GetUserLikes(LikesParams likesParams)
    {
        var usersQuery = _context.Users.OrderBy(x => x.UserName).AsQueryable();
        var likesQuery = _context.UserLikes.AsQueryable();

        if (likesParams.Predicate == "liked")
        {
            likesQuery = likesQuery.Where(x => x.SourceUserId == likesParams.UserId);
            usersQuery = likesQuery.Select(x => x.TargetUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likesQuery = likesQuery.Where(x => x.TargetUserId == likesParams.UserId);
            usersQuery = likesQuery.Select(x => x.SourceUser);
        }

        var likedUsers = usersQuery.Select(user => new LikesDTO
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.BirthDate.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfile)!.Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikesDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        var users = await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        return users!;
    }

    public async Task<bool> SaveAllAsync()
    => await _context.SaveChangesAsync() > 0;
}
