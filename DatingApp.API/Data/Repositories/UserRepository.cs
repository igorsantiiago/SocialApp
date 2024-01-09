using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
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
        => await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<AppUser?> GetUserByUsername(string username)
        => await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == username);

    public async Task<IEnumerable<AppUserDTO>> GetAppUsersDtoAsync()
        => await _context.Users.ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task<AppUserDTO?> GetAppUserDtoByUsername(string username)
        => await _context.Users.Where(x => x.UserName == username)
            .ProjectTo<AppUserDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<bool> SaveAllAsync()
        => await _context.SaveChangesAsync() > 0;

    public void Update(AppUser user)
        => _context.Entry(user).State = EntityState.Modified;
}
