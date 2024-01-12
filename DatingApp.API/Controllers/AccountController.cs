using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API;

public class AccountController : BaseApiController
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDTO>> RegisterAccount(RegisterAccountDTO registerDto)
    {
        if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserResponseDTO
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDTO>> LoginAccount(LoginDTO loginDto)
    {
        var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

        if (user is null)
            return Unauthorized("Username or Password Invalid.");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
                return Unauthorized("Username or Password Invalid");
        }

        return new UserResponseDTO
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfile)?.Url!
        };
    }

    private async Task<bool> UserExists(string username)
        => await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
}
