using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    public UserController(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("search/list")]
    public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsers()
    {
        var users = await _repository.GetAppUsersDtoAsync();

        return Ok(users);

    }
    [HttpGet("search/id/{id}")]
    public async Task<ActionResult<AppUserDTO>> GetUserById(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user is null)
            return Ok("Usuário não encontrado.");

        return _mapper.Map<AppUserDTO>(user);
    }

    [HttpGet("search/username/{username}")]
    public async Task<ActionResult<AppUserDTO>> GetUserByUsername(string username)
    {
        var user = await _repository.GetAppUserDtoByUsername(username);
        return user!;
    }
}
