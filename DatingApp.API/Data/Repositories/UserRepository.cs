using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public UserRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
        => await _context.Users.Include(x => x.Photos).AsNoTracking().ToListAsync();

    public async Task<AppUser?> GetUserByIdAsync(int id)
        => await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<AppUser?> GetUserByUsername(string username)
        => await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == username);

    public async Task<PagedList<AppUserDTO>> GetAppUsersDtoAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();
        query = query.Where(username => username.UserName != userParams.CurrentUsername);
        query = query.Where(gender => gender.Gender == userParams.Gender);
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.LastActivity)
        };

        var earliestBirthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaximumAge - 1));
        var latestBirthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinimumAge));

        query = query.Where(x => x.BirthDate >= earliestBirthdate && x.BirthDate <= latestBirthdate);

        return await PagedList<AppUserDTO>.CreateAsync(query.AsNoTracking().ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
    }
    public async Task<AppUserDTO?> GetAppUserDtoByUsername(string username, bool isCurrentUser)
    {
        var query = _context.Users.Where(x => x.UserName == username)
                .ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider)
                .AsQueryable();

        if (isCurrentUser) query = query.IgnoreQueryFilters();

        return await query.FirstOrDefaultAsync();
    }

    public void Update(AppUser user)
        => _context.Entry(user).State = EntityState.Modified;

    public async Task<string> GetUserGender(string username)
    {
        var gender = await _context.Users.Where(x => x.UserName == username).Select(x => x.Gender).FirstOrDefaultAsync();
        return gender!;
    }

    public async Task<AppUser> GetUserByPhotoId(int photoId)
    {
        return await _context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(p => p.Photos.Any(p => p.Id == photoId))
            .FirstOrDefaultAsync();
    }
}
