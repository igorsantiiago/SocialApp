using System.Security.Cryptography;
using System.Text;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public AccountController(AppDbContext context, ITokenService tokenService, IMapper mapper)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDTO>> RegisterAccount(RegisterAccountDTO registerDto)
    {
        if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(registerDto);

        using var hmac = new HMACSHA512();

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserResponseDTO
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs
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
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfile)?.Url!,
            KnownAs = user.KnownAs
        };
    }

    private async Task<bool> UserExists(string username)
        => await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
}
