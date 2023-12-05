using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly AppDbContext _context;
    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("search/list")]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        return users;
    }

    [AllowAnonymous]
    [HttpGet("search/{id}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return user!;
    }
}
