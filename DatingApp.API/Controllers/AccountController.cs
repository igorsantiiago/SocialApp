using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDTO>> RegisterAccount(RegisterAccountDTO registerDto)
    {
        if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Usuário");
        if (!roleResult.Succeeded)
            return BadRequest(result.Errors);

        return new UserResponseDTO
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDTO>> LoginAccount(LoginDTO loginDto)
    {
        var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

        if (user is null)
            return Unauthorized("Username or Password Invalid.");

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
            return Unauthorized("Username or Password Invalid.");

        return new UserResponseDTO
        {
            Username = user.UserName!,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfile)?.Url!,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
        => await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
}
